import Cookies from 'js-cookie';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5050';

interface RequestOptions extends RequestInit {
    token?: string;
    _retry?: boolean;
}

interface QueuedRequest {
    resolve: (token: string) => void;
    reject: (error: any) => void;
}

class ApiClient {
    private baseUrl: string;
    private isRefreshing = false;
    private failedQueue: QueuedRequest[] = [];

    constructor(baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    private processQueue(error: any, token: string | null = null) {
        this.failedQueue.forEach(prom => {
            if (error) {
                prom.reject(error);
            } else if (token) {
                prom.resolve(token);
            }
        });
        this.failedQueue = [];
    }

    private async request<T>(
        endpoint: string,
        options: RequestOptions = {}
    ): Promise<T> {
        let { token, _retry, ...fetchOptions } = options;

        // Auto-inject token from cookies if not provided
        if (!token && typeof window !== 'undefined') {
            token = Cookies.get('accessToken');
        }

        const headers: Record<string, string> = {
            ...(fetchOptions.headers as Record<string, string>),
        };

        // If body is NOT FormData, set JSON content type
        if (!(fetchOptions.body instanceof FormData)) {
            headers['Content-Type'] = 'application/json';
        }

        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }

        try {
            const response = await fetch(`${this.baseUrl}${endpoint}`, {
                ...fetchOptions,
                headers,
            });

            if (!response.ok) {
                // Handle 401 Unauthorized
                if (response.status === 401 && !_retry) {
                    if (this.isRefreshing) {
                        // Queue this request if a refresh is already in progress
                        return new Promise<T>((resolve, reject) => {
                            this.failedQueue.push({
                                resolve: (newToken: string) => {
                                    // Retry the original request with the new token
                                    this.request<T>(endpoint, { ...options, token: newToken, _retry: true })
                                        .then(resolve)
                                        .catch(reject);
                                },
                                reject: (err) => reject(err),
                            });
                        });
                    }

                    // Start Refresh Process
                    this.isRefreshing = true;

                    try {
                        const refreshToken = Cookies.get('refreshToken');
                        if (!refreshToken) throw new Error('No refresh token available');

                        // Direct fetch to avoid recursion
                        const refreshResponse = await fetch(`${this.baseUrl}/api/identity/refresh`, {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify({ refreshToken }),
                        });

                        if (!refreshResponse.ok) throw new Error('Refresh failed');

                        const authData = await refreshResponse.json();
                        const { accessToken, refreshToken: newRefreshToken, userId } = authData;

                        // Update Cookies
                        const expiresInDays = 7;
                        const secure = process.env.NODE_ENV === 'production';

                        Cookies.set('accessToken', accessToken, { expires: 1, sameSite: 'Strict', secure });
                        Cookies.set('refreshToken', newRefreshToken, { expires: expiresInDays, sameSite: 'Strict', secure });
                        if (userId) Cookies.set('userId', userId, { expires: expiresInDays, sameSite: 'Strict', secure });

                        // Process Queue
                        this.processQueue(null, accessToken);
                        this.isRefreshing = false;

                        // Retry original request
                        return this.request<T>(endpoint, { ...options, token: accessToken, _retry: true });

                    } catch (err) {
                        this.processQueue(err, null);
                        this.isRefreshing = false;

                        // Logout on failure
                        Cookies.remove('accessToken');
                        Cookies.remove('refreshToken');
                        Cookies.remove('userId');

                        if (typeof window !== 'undefined' && !window.location.pathname.includes('/login')) {
                            window.location.href = '/login';
                        }

                        throw new Error('Session expired');
                    }
                }

                // Handle other errors
                const error = await response.json().catch(() => ({ error: 'Request failed' }));
                throw new Error(error.error || `HTTP ${response.status}`);
            }

            return response.json();
        } catch (error) {
            throw error;
        }
    }

    async get<T>(endpoint: string, token?: string): Promise<T> {
        return this.request<T>(endpoint, { method: 'GET', token });
    }

    async post<T>(endpoint: string, data: unknown, token?: string): Promise<T> {
        return this.request<T>(endpoint, {
            method: 'POST',
            body: data instanceof FormData ? data : JSON.stringify(data),
            token,
        });
    }

    async put<T>(endpoint: string, data: unknown, token?: string): Promise<T> {
        return this.request<T>(endpoint, {
            method: 'PUT',
            body: data instanceof FormData ? data : JSON.stringify(data),
            token,
        });
    }

    async patch<T>(endpoint: string, data: unknown, token?: string): Promise<T> {
        return this.request<T>(endpoint, {
            method: 'PATCH',
            body: JSON.stringify(data),
            token,
        });
    }

    async delete<T>(endpoint: string, token?: string): Promise<T> {
        return this.request<T>(endpoint, { method: 'DELETE', token });
    }
}

export const apiClient = new ApiClient(API_BASE_URL);
