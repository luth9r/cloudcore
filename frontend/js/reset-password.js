import { ApiClient } from './api.js';
import { I18n } from './translations.js';

// Initialize API and i18n
const api = new ApiClient();
const i18n = new I18n();

// Initialize translations
i18n.updateUI();

// Language switcher
const languageBtn = document.getElementById('languageBtn');
if (languageBtn) {
    languageBtn.addEventListener('click', () => {
        i18n.switchLanguage();
        location.reload();
    });
}

// Get token from URL
const urlParams = new URLSearchParams(window.location.search);
const token = urlParams.get('token');

// Check if token exists
if (!token) {
    showMessage(i18n.t('invalidResetToken'), 'error');
    setTimeout(() => {
        window.location.href = 'login.html';
    }, 3000);
}

// Initialize theme switcher
const themeBtn = document.getElementById('themeBtn');
const themeIcon = themeBtn.querySelector('.theme-icon');

function updateThemeIcon(theme) {
    themeIcon.textContent = theme === 'dark' ? 'light_mode' : 'dark_mode';
}

const currentTheme = document.documentElement.getAttribute('data-theme');
updateThemeIcon(currentTheme);

themeBtn.addEventListener('click', () => {
    const currentTheme = document.documentElement.getAttribute('data-theme');
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';

    document.documentElement.setAttribute('data-theme', newTheme);
    localStorage.setItem('cloudcore-theme', newTheme);
    updateThemeIcon(newTheme);
});

// Password visibility toggles
document.querySelectorAll('.toggle-password').forEach((button) => {
    button.addEventListener('click', function () {
        const wrapper = this.closest('.password-wrapper');
        const input = wrapper.querySelector('input');
        const icon = this.querySelector('.material-symbols-outlined');

        if (input.type === 'password') {
            input.type = 'text';
            icon.textContent = 'visibility';
        } else {
            input.type = 'password';
            icon.textContent = 'visibility_off';
        }
    });
});

// Form submission
const resetPasswordForm = document.getElementById('resetPasswordForm');
const resetBtn = document.getElementById('resetBtn');

resetPasswordForm.addEventListener('submit', async (e) => {
    e.preventDefault();

    const newPassword = document.getElementById('newPassword').value;
    const confirmPassword = document.getElementById('confirmPassword').value;

    // Validation
    if (newPassword.length < 6) {
        showMessage(i18n.t('passwordTooShort'), 'error');
        return;
    }

    if (newPassword !== confirmPassword) {
        showMessage(i18n.t('passwordsDoNotMatch'), 'error');
        return;
    }

    try {
        resetBtn.disabled = true;
        resetBtn.textContent = 'Resetting...';

        await api.resetPassword(token, newPassword);

        showMessage(i18n.t('passwordResetSuccess'), 'success');
        setTimeout(() => {
            window.location.href = 'login.html';
        }, 2000);
    } catch (error) {
        console.error('Reset password error:', error);
        showMessage(i18n.t('passwordResetFailed'), 'error');
    } finally {
        resetBtn.disabled = false;
        resetBtn.textContent = 'Reset Password';
    }
});

function showMessage(message, type) {
    const messageEl =
        type === 'error' ? document.getElementById('error-message') : document.getElementById('success-message');

    // Hide other message
    const otherEl =
        type === 'error' ? document.getElementById('success-message') : document.getElementById('error-message');
    otherEl.style.display = 'none';

    messageEl.textContent = message;
    messageEl.style.display = 'flex';
}
