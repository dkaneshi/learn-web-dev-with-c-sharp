// Learn C# for Web Dev - Site JavaScript

// Global utilities
window.LearnCSharp = {
    // Show toast notifications
    showToast: function(message, type = 'info') {
        const toast = document.createElement('div');
        toast.className = `fixed top-4 right-4 z-50 p-4 rounded-lg shadow-lg text-white ${this.getToastColor(type)}`;
        toast.textContent = message;
        
        document.body.appendChild(toast);
        
        setTimeout(() => {
            toast.classList.add('opacity-0', 'transition-opacity');
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    },
    
    getToastColor: function(type) {
        switch(type) {
            case 'success': return 'bg-green-500';
            case 'error': return 'bg-red-500';
            case 'warning': return 'bg-yellow-500';
            default: return 'bg-blue-500';
        }
    },
    
    // Format time duration
    formatDuration: function(minutes) {
        if (minutes < 60) {
            return `${minutes}m`;
        }
        const hours = Math.floor(minutes / 60);
        const remainingMinutes = minutes % 60;
        return remainingMinutes > 0 ? `${hours}h ${remainingMinutes}m` : `${hours}h`;
    },
    
    // Copy code to clipboard
    copyCode: function(code) {
        navigator.clipboard.writeText(code).then(() => {
            this.showToast('Code copied to clipboard!', 'success');
        }).catch(() => {
            this.showToast('Failed to copy code', 'error');
        });
    }
};

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Add copy buttons to code blocks
    document.querySelectorAll('pre code').forEach(block => {
        const button = document.createElement('button');
        button.className = 'absolute top-2 right-2 bg-gray-600 hover:bg-gray-700 text-white px-2 py-1 rounded text-xs';
        button.textContent = 'Copy';
        button.onclick = () => LearnCSharp.copyCode(block.textContent);
        
        block.parentElement.style.position = 'relative';
        block.parentElement.appendChild(button);
    });
    
    // Auto-hide alerts after 5 seconds
    document.querySelectorAll('.alert').forEach(alert => {
        setTimeout(() => {
            alert.style.opacity = '0';
            alert.style.transform = 'translateY(-10px)';
            setTimeout(() => alert.remove(), 300);
        }, 5000);
    });
});

// Keyboard shortcuts
document.addEventListener('keydown', function(e) {
    // Ctrl/Cmd + Enter to submit forms
    if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
        const activeElement = document.activeElement;
        if (activeElement && activeElement.form) {
            const submitButton = activeElement.form.querySelector('button[type="submit"], input[type="submit"]');
            if (submitButton) {
                submitButton.click();
            }
        }
    }
    
    // Escape to close modals
    if (e.key === 'Escape') {
        document.querySelectorAll('.modal').forEach(modal => {
            modal.style.display = 'none';
        });
    }
});

// Progress bar animation
function animateProgressBar(element, targetPercentage) {
    let current = 0;
    const increment = targetPercentage / 50;
    
    const timer = setInterval(() => {
        current += increment;
        if (current >= targetPercentage) {
            current = targetPercentage;
            clearInterval(timer);
        }
        element.style.width = `${current}%`;
    }, 20);
}

// Initialize progress bars on page load
document.addEventListener('DOMContentLoaded', function() {
    document.querySelectorAll('.progress-bar').forEach(bar => {
        const target = parseInt(bar.dataset.progress) || 0;
        animateProgressBar(bar, target);
    });
});