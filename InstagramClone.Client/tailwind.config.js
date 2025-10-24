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
          dark: '#121212',
        },
        'instagram-border': {
          light: '#dbdbdb',
          DEFAULT: '#dbdbdb',
          dark: '#262626',
        },
        'instagram-text': {
          primary: {
            light: '#262626',
            dark: '#fafafa',
          },
          secondary: {
            light: '#737373',
            dark: '#a8a8a8',
          }
        },
        'instagram-bg': {
          primary: {
            light: '#ffffff',
            dark: '#000000',
          },
          secondary: {
            light: '#fafafa',
            dark: '#121212',
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

