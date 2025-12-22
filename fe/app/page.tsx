export default function Home() {
  return (
    <main className="relative z-10 w-full">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-20">
        {/* Hero Section */}
        <section className="text-center mb-32 animate-[var(--animate-fade-up)]">
          <div className="mb-8">
            <h1 className="text-7xl sm:text-8xl font-black mb-6 leading-tight">
              <span className="bg-gradient-to-r from-[var(--color-primary)] to-[var(--color-secondary)] bg-clip-text text-transparent">Sonixy</span>
            </h1>
            <p className="text-3xl sm:text-4xl font-bold text-white mb-4">
              Modern Social Platform
            </p>
            <p className="text-lg sm:text-xl text-[var(--color-text-secondary)] max-w-3xl mx-auto leading-relaxed">
              A production-ready microservice architecture demonstrating clean code,
              scalable design patterns, and premium UI/UX.
            </p>
          </div>

          <div className="flex flex-col sm:flex-row gap-4 justify-center items-center mt-12">
            <button className="btn-primary text-lg px-10 py-4 shadow-[var(--shadow-neon)] hover:shadow-[0_0_40px_rgba(0,229,255,0.6)] transition-all duration-300">
              Get Started 🚀
            </button>
            <button className="glass-base px-10 py-4 rounded-full font-semibold hover:bg-white/10 transition-all">
              View Docs 📚
            </button>
          </div>
        </section>

        {/* Features Grid */}
        <section className="grid sm:grid-cols-2 lg:grid-cols-3 gap-8 mb-32">
          <div
            className="glass-float rounded-2xl p-8 hover:-translate-y-2 transition-transform duration-300 animate-[var(--animate-fade-up)]"
            style={{ animationDelay: '0.1s' }}
          >
            <div className="w-16 h-16 mb-6 rounded-2xl bg-gradient-to-br from-[var(--color-primary)] to-blue-500 flex items-center justify-center text-4xl shadow-[var(--shadow-neon)]">
              🏗️
            </div>
            <h3 className="text-2xl font-bold mb-4 text-white">Microservices</h3>
            <p className="text-[var(--color-text-secondary)] leading-relaxed">
              Clean layered architecture with User, Post, and SocialGraph services
            </p>
          </div>

          <div
            className="glass-float rounded-2xl p-8 hover:-translate-y-2 transition-transform duration-300 animate-[var(--animate-fade-up)]"
            style={{ animationDelay: '0.2s' }}
          >
            <div className="w-16 h-16 mb-6 rounded-2xl bg-gradient-to-br from-[var(--color-secondary)] to-purple-500 flex items-center justify-center text-4xl shadow-[var(--shadow-soft)]">
              ⚡
            </div>
            <h3 className="text-2xl font-bold mb-4 text-white">Performance</h3>
            <p className="text-[var(--color-text-secondary)] leading-relaxed">
              Cursor pagination, MongoDB indexes, and optimized queries
            </p>
          </div>

          <div
            className="glass-float rounded-2xl p-8 hover:-translate-y-2 transition-transform duration-300 animate-[var(--animate-fade-up)]"
            style={{ animationDelay: '0.3s' }}
          >
            <div className="w-16 h-16 mb-6 rounded-2xl bg-gradient-to-br from-pink-500 to-[var(--color-primary)] flex items-center justify-center text-4xl shadow-[var(--shadow-soft)]">
              🎨
            </div>
            <h3 className="text-2xl font-bold mb-4 text-white">Premium Design</h3>
            <p className="text-[var(--color-text-secondary)] leading-relaxed">
              Glassmorphism, smooth animations, and cohesive design system
            </p>
          </div>
        </section>

        {/* Tech Stack */}
        <section className="glass-overlay rounded-3xl p-10 sm:p-12 shadow-[var(--shadow-lift)] animate-[var(--animate-scale-in)]">
          <h2 className="text-4xl sm:text-5xl font-black mb-12 text-center">
            <span className="bg-gradient-to-r from-white to-white/60 bg-clip-text text-transparent">Tech Stack</span>
          </h2>

          <div className="grid md:grid-cols-2 gap-12">
            <div className="space-y-6">
              <div className="flex items-center gap-3 mb-6">
                <div className="w-12 h-12 rounded-xl bg-gradient-to-br from-[var(--color-primary)] to-blue-500 flex items-center justify-center shadow-[var(--shadow-neon)]">
                  <span className="text-2xl">⚙️</span>
                </div>
                <h4 className="text-2xl font-bold text-white">Backend</h4>
              </div>
              <ul className="space-y-4">
                {['.NET 10 + ASP.NET Core', 'MongoDB with ObjectId', 'Repository + Specification Pattern', 'Swagger Documentation', 'gRPC Inter-Service Communication'].map(item => (
                  <li key={item} className="flex items-center gap-3 text-[var(--color-text-secondary)] text-lg">
                    <span className="text-[var(--color-primary)] text-xl">✓</span>
                    <span>{item}</span>
                  </li>
                ))}
              </ul>
            </div>

            <div className="space-y-6">
              <div className="flex items-center gap-3 mb-6">
                <div className="w-12 h-12 rounded-xl bg-gradient-to-br from-[var(--color-secondary)] to-purple-600 flex items-center justify-center shadow-[var(--shadow-soft)]">
                  <span className="text-2xl">💻</span>
                </div>
                <h4 className="text-2xl font-bold text-white">Frontend</h4>
              </div>
              <ul className="space-y-4">
                {['Next.js 15 (App Router)', 'TypeScript', 'Tailwind CSS v4', 'SEO Optimized', 'Premium Design System'].map(item => (
                  <li key={item} className="flex items-center gap-3 text-[var(--color-text-secondary)] text-lg">
                    <span className="text-[var(--color-secondary)] text-xl">✓</span>
                    <span>{item}</span>
                  </li>
                ))}
              </ul>
            </div>
          </div>
        </section>
      </div>
    </main>
  )
}
