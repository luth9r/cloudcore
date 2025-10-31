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

const newPasswordInput = document.getElementById('newPassword');
const confirmPasswordInput = document.getElementById('confirmPassword');

if (newPasswordInput) {
    newPasswordInput.addEventListener('input', () => {
        validatePassword(newPasswordInput.value);
    });
}

if (confirmPasswordInput) {
    confirmPasswordInput.addEventListener('input', () => {
        const newPassword = newPasswordInput.value;
        const confirmPassword = confirmPasswordInput.value;
        const matchHint = document.getElementById('password-match-hint');

        if (confirmPassword === '' && newPassword === '') {
            confirmPasswordInput.style.borderColor = 'var(--border-color)';
            if (matchHint) matchHint.style.display = 'none';
        } else if (newPassword === confirmPassword && confirmPassword !== '') {
            confirmPasswordInput.style.borderColor = 'var(--color-green)';
            if (matchHint) matchHint.style.display = 'none';
        } else if (confirmPassword !== '') {
            confirmPasswordInput.style.borderColor = 'var(--color-red)';
            if (matchHint) matchHint.style.display = 'flex';
        }
    });
}

/**
 * Validate password requirements in real-time
 */
function validatePassword(password) {
    const requirements = {
        length: password.length >= 8,
        lowercase: /[a-z]/.test(password),
        number: /\d/.test(password)
    };

    updateRequirement('req-length', requirements.length);
    updateRequirement('req-lowercase', requirements.lowercase);
    updateRequirement('req-number', requirements.number);
}

/**
 * Update requirement indicator UI
 */
function updateRequirement(elementId, isValid) {
    const element = document.getElementById(elementId);
    if (!element) return;

    const dot = element.querySelector('.req-dot');

    if (isValid) {
        element.classList.add('valid');
        element.style.color = 'var(--color-green)';
        if (dot) {
            dot.style.background = 'var(--color-green)';
            dot.style.boxShadow = '0 0 0 3px rgba(52, 168, 83, 0.2)';
            dot.style.transform = 'scale(1.3)';
        }
    } else {
        element.classList.remove('valid');
        element.style.color = 'var(--text-secondary)';
        if (dot) {
            dot.style.background = 'var(--color-red)';
            dot.style.boxShadow = 'none';
            dot.style.transform = 'scale(1)';
        }
    }
}

// ============================================================
// FORM SUBMISSION
// ============================================================

const resetPasswordForm = document.getElementById('resetPasswordForm');
const resetBtn = document.getElementById('resetBtn');

resetPasswordForm.addEventListener('submit', async (e) => {
    e.preventDefault();

    const newPassword = newPasswordInput.value;
    const confirmPassword = confirmPasswordInput.value;

    if (newPassword.length < 8) {
        showMessage(i18n.t('passwordTooShort'), 'error');
        return;
    }

    if (!/[a-z]/.test(newPassword)) {
        showMessage(i18n.t('passwordNeedsLowercase'), 'error');
        return;
    }

    if (!/\d/.test(newPassword)) {
        showMessage(i18n.t('passwordNeedsNumber'), 'error');
        return;
    }

    if (newPassword !== confirmPassword) {
        showMessage(i18n.t('passwordsDoNotMatch'), 'error');
        return;
    }

    try {
        resetBtn.disabled = true;
        resetBtn.textContent = i18n.t('resetting');

        await api.resetPassword(token, newPassword);

        showMessage(i18n.t('passwordResetSuccess'), 'success');
        setTimeout(() => {
            window.location.href = 'login.html';
        }, 2000);
    } catch (error) {
        console.error('Reset password error:', error);

        const msg = error.message?.toLowerCase() || '';
        let errorMessage = i18n.t('passwordResetFailed');

        if (msg.includes('expired') || msg.includes('invalid')) {
            errorMessage = i18n.t('invalidOrExpiredToken');
        } else if (error.message) {
            errorMessage = error.message;
        }

        showMessage(errorMessage, 'error');
    } finally {
        resetBtn.disabled = false;
        resetBtn.textContent = i18n.t('resetPassword');
    }
});

function showMessage(message, type) {
    const messageEl =
        type === 'error' ? document.getElementById('error-message') : document.getElementById('success-message');

    const otherEl =
        type === 'error' ? document.getElementById('success-message') : document.getElementById('error-message');

    otherEl.style.display = 'none';

    messageEl.textContent = message;
    messageEl.style.display = 'flex';
}
