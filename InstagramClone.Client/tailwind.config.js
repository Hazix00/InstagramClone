/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./**/*.{razor,html,cshtml}",
    "./**/*.cs"
  ],
  darkMode: 'class', // Use class-based dark mode for programmatic control
  theme: {
    extend: {
      colors: {
        'instagram-blue': '#0095f6',
        'instagram-gray': {
          light: '#fafafa',
          DEFAULT: '#fafafa',
          dark: '#0f172a',
        },
        'instagram-border': {
          light: '#dbdbdb',
          DEFAULT: '#dbdbdb',
          dark: '#1f2937',
        },
        'instagram-text': {
          primary: {
            light: '#262626',
            dark: '#e5e7eb',
          },
          secondary: {
            light: '#737373',
            dark: '#9ca3af',
          }
        },
        'instagram-bg': {
          primary: {
            light: '#ffffff',
            dark: '#111827',
          },
          secondary: {
            light: '#fafafa',
            dark: '#0f172a',
          }
        }
      },
      fontFamily: {
        'sans': ['Inter', 'system-ui', 'sans-serif'],
      }
    },
  },
  plugins: [],
}

