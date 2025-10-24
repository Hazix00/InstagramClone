# Tailwind CSS Setup

This project uses Tailwind CSS v4 with dark mode support.

## Installation

Tailwind CSS and its dependencies are already installed. If you need to reinstall:

```bash
cd InstagramClone.Client
npm install
```

## Building CSS

### One-time Build
```bash
npm run css:build
```

### Watch Mode (recommended for development)
```bash
npm run css:watch
```

## VS Code Tasks

You can use the following tasks from VS Code:
- **Build Tailwind CSS**: One-time build
- **Watch Tailwind CSS**: Watch mode for development
- **Run All with Tailwind (watch)**: Runs API, Client, and Tailwind in watch mode

## Dark Mode

The app automatically detects and follows the user's system theme preference (light/dark mode).

### Custom Colors

Instagram-themed colors are available with dark mode variants:

```css
/* Backgrounds */
bg-instagram-gray          /* Light: #fafafa, Dark: #121212 */
bg-white dark:bg-black     /* Primary backgrounds */

/* Borders */
border-instagram-border dark:border-instagram-border-dark

/* Text */
text-gray-900 dark:text-white         /* Primary text */
text-gray-500 dark:text-gray-400      /* Secondary text */

/* Brand */
bg-instagram-blue                      /* #0095f6 */
```

### Example Usage

```html
<div class="bg-white dark:bg-black border border-instagram-border dark:border-instagram-border-dark">
  <h1 class="text-gray-900 dark:text-white">Title</h1>
  <p class="text-gray-500 dark:text-gray-400">Description</p>
</div>
```

## Configuration

Tailwind configuration is in `tailwind.config.js`:
- Custom Instagram color palette
- Dark mode set to 'media' (follows system preference)
- Content paths for Razor and C# files

## Source Files

- **Input**: `Styles/app.css` (source file with Tailwind directives)
- **Output**: `wwwroot/css/app.css` (compiled CSS, included in .gitignore)
- **Config**: `tailwind.config.js`
- **Package**: `package.json` (npm scripts and dependencies)

## Important Notes

1. Always run `npm run css:build` before committing if you've made changes to:
   - Tailwind configuration
   - Custom CSS in `Styles/app.css`
   - Added new Tailwind classes in your components

2. The compiled `wwwroot/css/app.css` should be excluded from version control (already in .gitignore with `*.Designer.cs` exclusion)

3. For production builds, the CSS is automatically purged to remove unused styles based on your content files.

