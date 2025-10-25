let dotNetHelper = null;
let isLoading = false;
let scrollHandler = null;

export function initInfiniteScroll(dotNetObj) {
    dotNetHelper = dotNetObj;
    
    scrollHandler = () => {
        if (isLoading) return;
        
        const scrollPosition = window.innerHeight + window.scrollY;
        const threshold = document.documentElement.scrollHeight - 500; // 500px before bottom
        
        if (scrollPosition >= threshold) {
            isLoading = true;
            dotNetHelper.invokeMethodAsync('LoadMore')
                .then(() => {
                    isLoading = false;
                })
                .catch(() => {
                    isLoading = false;
                });
        }
    };
    
    window.addEventListener('scroll', scrollHandler);
}

export function cleanup() {
    if (scrollHandler) {
        window.removeEventListener('scroll', scrollHandler);
        scrollHandler = null;
    }
    dotNetHelper = null;
}

