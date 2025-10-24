# Theme Management Guide

## How It Works

The app uses **programmatic theme control** with these features:

### 🎨 Three Theme Options

1. **Light Mode** - Always use light theme
2. **Dark Mode** - Always use dark theme  
3. **System** (Default) - Automatically follows the user's system/browser preference

### 💾 Persistence Strategy

**On first visit (no saved preference):**
- Automatically detects system preference
- Uses dark mode if system is set to dark
- Uses light mode if system is set to light

**After user selects a theme:**
- Choice is saved to `localStorage`
- Persists across browser sessions
- Applied immediately without page reload

**When user selects "System":**
- Removes saved preference from `localStorage`
- Falls back to system preference detection
- Dynamically responds to system theme changes

## 🔧 Technical Implementation

### 1. JavaScript Theme Manager (`index.html`)

```javascript
window.themeManager = {
    key: 'theme',
    
    // Get current theme (saved or system default)
    get: function () {
        var saved = localStorage.getItem(this.key);
        if (saved) return saved;
        // Default to system preference if no saved theme
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    },
    
    // Save theme to localStorage and apply
    set: function (theme) {
        localStorage.setItem(this.key, theme);
        this.apply(theme);
    },
    
    // Apply theme by adding/removing 'dark' class on <html>
    apply: function (theme) {
        var themeToUse = theme || this.get();
        if (themeToUse === 'dark') {
            document.documentElement.classList.add('dark');
        } else {
            document.documentElement.classList.remove('dark');
        }
    },
    
    // Toggle between light and dark
    toggle: function () {
        var current = this.get();
        var newTheme = current === 'dark' ? 'light' : 'dark';
        this.set(newTheme);
        return newTheme;
    }
};
```

### 2. Tailwind Configuration

```javascript
// tailwind.config.js
module.exports = {
  darkMode: 'class', // Use class-based dark mode for programmatic control
  // ... rest of config
}
```

### 3. Settings Page Integration

Users can change theme in **Settings > Theme**:
- Dropdown with Light/Dark/System options
- Changes apply immediately
- No page reload required

### 4. Theme Toggle Component (Optional)

You can add `<ThemeToggle />` anywhere in your app for quick theme switching:

```razor
<ThemeToggle />
```

## 🎯 Usage in Components

### Apply Dark Mode to Elements

Use Tailwind's `dark:` variant:

```html
<!-- Background colors -->
<div class="bg-white dark:bg-black">

<!-- Text colors -->
<p class="text-gray-900 dark:text-white">

<!-- Borders -->
<div class="border-gray-300 dark:border-gray-700">

<!-- Instagram theme colors -->
<div class="bg-instagram-gray dark:bg-instagram-gray-dark">
<div class="border-instagram-border dark:border-instagram-border-dark">
```

### Custom Instagram Colors

Available theme-aware colors:

```css
/* Backgrounds */
bg-instagram-gray              /* #fafafa in light, #121212 in dark */
bg-instagram-gray-light        /* Always #fafafa */
bg-instagram-gray-dark         /* Always #121212 */

/* Borders */
border-instagram-border        /* #dbdbdb in light, #262626 in dark */
border-instagram-border-light  /* Always #dbdbdb */
border-instagram-border-dark   /* Always #262626 */

/* Brand color (same in both modes) */
bg-instagram-blue              /* #0095f6 */
```

## 📱 Programmatic Access from C#

### Get Current Theme

```csharp
@inject IJSRuntime JS

var currentTheme = await JS.InvokeAsync<string>("themeManager.get");
// Returns: "light", "dark"
```

### Set Theme

```csharp
// Set to dark mode
await JS.InvokeVoidAsync("themeManager.set", "dark");

// Set to light mode
await JS.InvokeVoidAsync("themeManager.set", "light");
```

### Toggle Theme

```csharp
var newTheme = await JS.InvokeAsync<string>("themeManager.toggle");
// Returns the new theme: "light" or "dark"
```

### Handle "System" Option

```csharp
// Remove saved preference (use system default)
await JS.InvokeVoidAsync("localStorage.removeItem", "theme");

// Detect and apply system preference
var isDark = await JS.InvokeAsync<bool>(
    "window.matchMedia('(prefers-color-scheme: dark)').matches"
);
await JS.InvokeVoidAsync("themeManager.apply", isDark ? "dark" : "light");
```

## 🚀 Best Practices

1. **Always use dark mode variants** when styling components
2. **Test both themes** during development
3. **Use semantic colors** (gray-900/white) instead of hardcoded colors
4. **Avoid theme-specific logic** in C# code - let CSS handle it
5. **Run `npm run css:watch`** during development for auto-rebuild

## 🔍 Debugging

Check current theme state in browser console:

```javascript
// Get current theme
console.log(themeManager.get());

// Check if dark class is applied
console.log(document.documentElement.classList.contains('dark'));

// Check localStorage
console.log(localStorage.getItem('theme'));
```

## 📦 Files Modified

- `tailwind.config.js` - Dark mode configuration
- `wwwroot/index.html` - Theme manager JavaScript
- `Pages/Settings/Settings.razor` - Theme selector UI
- `Pages/Settings/Settings.razor.cs` - Theme change handlers
- `Components/ThemeToggle.razor` - Optional toggle button

## ⚡ Performance

- Theme is applied **before first paint** (no flash)
- No JavaScript bundle required for theme detection
- localStorage is fast (synchronous)
- Class toggle is instant (no DOM repaint)

