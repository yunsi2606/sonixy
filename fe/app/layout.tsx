import './globals.css'
import type { Metadata } from 'next'
import { Inter } from 'next/font/google'
import { Providers } from './providers'
import { GlassShell } from '@/components/layout/GlassShell'

const inter = Inter({ subsets: ['latin'], variable: '--font-sans' })

export const metadata: Metadata = {
  title: 'Sonixy - Modern Social Platform',
  description: 'Connect, share, and express yourself on Sonixy',
  openGraph: {
    title: 'Sonixy - Modern Social Platform',
    description: 'Connect, share, and express yourself',
    type: 'website',
  },
  twitter: {
    card: 'summary_large_image',
    title: 'Sonixy',
    description: 'Connect, share, and express yourself',
  },
}

import { GlobalChatWidget } from '@/components/chat/GlobalChatWidget';

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="en" className={inter.variable}>
      <body>
        <Providers>
          <GlassShell>
            {children}
          </GlassShell>
          <GlobalChatWidget />
        </Providers>
      </body>
    </html>
  )
}
