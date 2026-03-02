/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        kyntus: {
          dark: '#0f172a',
          blue: '#1e3a5f',
          accent: '#3b82f6',
        },
        shift: {
          a: '#10b981',
          b: '#3b82f6',
          c: '#8b5cf6',
          d: '#f59e0b',
        },
        leave: {
          approved: '#60a5fa',
          maladie: '#f43f5e',
        }
      },
      borderRadius: {
        '2xl': '16px',
        '3xl': '24px',
      },
      backdropBlur: {
        xs: '2px',
      },
      boxShadow: {
        'glass': '0 8px 32px 0 rgba(31, 38, 135, 0.15)',
      }
    },
  },
  plugins: [],
}
