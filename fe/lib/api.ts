const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5050';

interface RequestOptions extends RequestInit {
    token?: string;
}

class ApiClient {
    private baseUrl: string;

    constructor(baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    private async request<T>(
        endpoint: string,
        options: RequestOptions = {}
    ): Promise<T> {
        let { token, ...fetchOptions } = options;

        // Auto-inject token from cookies if not provided
        if (!token && typeof window !== 'undefined') {
            const Cookies = (await import('js-cookie')).default;
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

        const response = await fetch(`${this.baseUrl}${endpoint}`, {
            ...fetchOptions,
            headers,
        });

        if (!response.ok) {
            const error = await response.json().catch(() => ({ error: 'Request failed' }));
            throw new Error(error.error || `HTTP ${response.status}`);
        }

        return response.json();
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
