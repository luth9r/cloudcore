import { I18n } from './translations.js';
import { ApiClient } from './api.js';

class AuthManager {
    constructor() {
        this.i18n = new I18n();
        this.api = new ApiClient();
        this.checkExistingAuth();
    }

    parseJwt(token) {
        try {
            const base64Url = token.split('.')[1];
            const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            const jsonPayload = decodeURIComponent(
                atob(base64)
                    .split('')
                    .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                    .join('')
            );
            return JSON.parse(jsonPayload);
        } catch (e) {
            return null;
        }
    }

    checkExistingAuth() {
        const token = localStorage.getItem('cloudcore_token');
        if (token) {
            const payload = this.parseJwt(token);
            if (payload) {
                if (payload.isEmailVerified) {
                    console.log('User email verified, redirecting...');
                    window.location.href = 'index.html';
                    return true;
                } else {
                    console.log('User email not verified.');
                    return false;
                }
            } else {
                localStorage.removeItem('cloudcore_token');
            }
        }
        return false;
    }

    showError(message) {
        const errorDiv = document.getElementById('error-message');
        if (errorDiv) {
            errorDiv.textContent = message;
            errorDiv.style.display = 'block';
        }
        this.hideSuccess();
    }

    showSuccess(message) {
        const successDiv = document.getElementById('success-message');
        if (successDiv) {
            successDiv.textContent = message;
            successDiv.style.display = 'block';
        }
        this.hideError();
    }

    hideError() {
        const errorDiv = document.getElementById('error-message');
        if (errorDiv) {
            errorDiv.style.display = 'none';
        }
    }

    hideSuccess() {
        const successDiv = document.getElementById('success-message');
        if (successDiv) {
            successDiv.style.display = 'none';
        }
    }

    hideMessages() {
        this.hideError();
        this.hideSuccess();
    }

    async handleLogin(username, password, button) {
        button.disabled = true;
        button.textContent = this.i18n.t('signingIn');
        this.hideMessages();

        try {
            const data = await this.api.login(username, password);

            // Store authentication data
            this.api.setAuthToken(data.token);
            localStorage.setItem(
                'cloudcore_user',
                JSON.stringify({
                    id: data.userId,
                    username: data.username,
                    email: data.email
                })
            );

            console.log('🎉 Login successful!');

            const welcomeMsg = this.i18n.t('welcomeBack').replace('{username}', data.username);
            this.showSuccess(welcomeMsg);

            // Redirect after 1 second
            setTimeout(() => {
                window.location.href = 'index.html';
            }, 1000);
        } catch (error) {
            console.error('Login error:', error);

            let errorMessage = this.i18n.t('signInFailed');
            if (error.message.toLowerCase().includes('invalid')) {
                errorMessage = this.i18n.t('invalidCredentials');
            } else if (error.message.toLowerCase().includes('unauthorized')) {
                errorMessage = this.i18n.t('invalidCredentials');
            } else if (error.message) {
                errorMessage = this.i18n.t('signInFailed');
            }

            this.showError(errorMessage);
        } finally {
            button.disabled = false;
            button.textContent = this.i18n.t('signIn');
        }
    }

    async handleRegister(username, email, password, confirmPassword, button) {
        // Validate passwords match
        if (password !== confirmPassword) {
            this.showError(this.i18n.t('passwordsNoMatch'));
            return;
        }

        button.disabled = true;
        button.textContent = this.i18n.t('creatingAccount');
        this.hideMessages();

        try {
            const data = await this.api.register(username, email, password);

            console.log('🎉 Registration successful!');

            this.showEmailVerificationModal();
        } catch (error) {
            console.error('Registration error:', error);

            let errorMessage = this.i18n.t('registrationFailed');
            if (error.message) {
                errorMessage = error.message;
            }

            this.showError(errorMessage);
        } finally {
            button.disabled = false;
            button.textContent = this.i18n.t('createAccount');
        }
    }

    setupThemeSwitcher() {
        const themeBtn = document.getElementById('themeBtn');
        const themeIcon = document.querySelector('.theme-icon');

        if (themeBtn) {
            // Set initial icon based on current theme
            const currentTheme = document.documentElement.getAttribute('data-theme');
            if (themeIcon) {
                themeIcon.textContent = currentTheme === 'dark' ? 'light_mode' : 'dark_mode';
            }

            themeBtn.addEventListener('click', () => {
                console.log('Theme switch clicked');
                const currentTheme = document.documentElement.getAttribute('data-theme');
                const newTheme = currentTheme === 'dark' ? 'light' : 'dark';

                document.documentElement.setAttribute('data-theme', newTheme);
                localStorage.setItem('cloudcore-theme', newTheme);

                // Update icon
                if (themeIcon) {
                    themeIcon.textContent = newTheme === 'dark' ? 'light_mode' : 'dark_mode';
                }

                console.log('Theme switched to:', newTheme);
            });
        }
    }

    setupLanguageSwitcher() {
        const languageBtn = document.getElementById('languageBtn');
        if (languageBtn) {
            languageBtn.addEventListener('click', () => {
                console.log('Language switch clicked');
                this.i18n.switchLanguage();
                location.reload();
            });
        }
    }

    setupPasswordVisibilityToggles() {
        document.querySelectorAll('.toggle-password').forEach((button) => {
            button.addEventListener('click', function () {
                // Find the input field (previous sibling)
                const wrapper = this.parentElement;
                const input = wrapper.querySelector('input');
                const icon = this.querySelector('.material-symbols-outlined');

                // Add animation class
                this.classList.add('changing');

                // Toggle password visibility
                if (input.type === 'password') {
                    input.type = 'text';
                    icon.textContent = 'visibility';
                    this.setAttribute('aria-label', 'Hide password');
                } else {
                    input.type = 'password';
                    icon.textContent = 'visibility_off';
                    this.setAttribute('aria-label', 'Show password');
                }

                // Remove animation class after transition
                setTimeout(() => {
                    this.classList.remove('changing');
                }, 150);

                // Keep focus on input
                input.focus();
            });
        });
    }

    initializeLoginPage() {
        this.setupPasswordVisibilityToggles();
        this.setupThemeSwitcher();
        this.i18n.updateUI();
        this.setupLanguageSwitcher();

        const form = document.getElementById('loginForm');
        if (form) {
            setTimeout(() => {
                form.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }, 100);
        }

        document.getElementById('loginForm')?.addEventListener('submit', async (e) => {
            e.preventDefault();

            const username = document.getElementById('username').value;
            const password = document.getElementById('password').value;
            const button = document.getElementById('loginBtn');

            await this.handleLogin(username, password, button);
        });
    }

    initializeRegisterPage() {
        this.setupPasswordVisibilityToggles();
        this.setupThemeSwitcher();
        this.i18n.updateUI();
        this.setupLanguageSwitcher();

        const form = document.getElementById('registerForm');
        if (form) {
            setTimeout(() => {
                form.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }, 100);
        }

        // Real-time password confirmation validation
        document.getElementById('confirmPassword')?.addEventListener('input', function () {
            const password = document.getElementById('password').value;
            const confirmPassword = this.value;

            if (confirmPassword && password !== confirmPassword) {
                this.style.borderColor = '#d93025';
            } else {
                this.style.borderColor = '#dadce0';
            }
        });

        document.getElementById('registerForm')?.addEventListener('submit', async (e) => {
            e.preventDefault();

            const username = document.getElementById('username').value;
            const email = document.getElementById('email').value;
            const password = document.getElementById('password').value;
            const confirmPassword = document.getElementById('confirmPassword').value;
            const button = document.getElementById('registerBtn');

            await this.handleRegister(username, email, password, confirmPassword, button);
        });
    }

    showEmailVerificationModal() {
        const modal = document.getElementById('emailVerificationModal');
        if (modal) {
            modal.style.display = 'flex';
        }
    }
}

// Export initialization functions
export function initLogin() {
    const authManager = new AuthManager();
    authManager.initializeLoginPage();
}

export function initRegister() {
    const authManager = new AuthManager();
    authManager.initializeRegisterPage();
}

// Auto-initialize based on current page
document.addEventListener('DOMContentLoaded', () => {
    if (document.getElementById('loginForm')) {
        initLogin();
    } else if (document.getElementById('registerForm')) {
        initRegister();
    }

    const modal = document.getElementById('emailVerificationModal');
    const closeBtn = document.getElementById('emailVerificationCloseBtn');
    const okBtn = document.getElementById('modalOkBtn');

    if (closeBtn) closeBtn.onclick = () => (modal.style.display = 'none');
    if (okBtn)
        okBtn.onclick = () => {
            modal.style.display = 'none';
            setTimeout(() => {
                window.location.href = 'login.html';
            }, 500);
        };
    window.onclick = (e) => {
        if (e.target === modal) modal.style.display = 'none';
    };
});

const forgotPasswordLink = document.getElementById('forgotPasswordLink');

if (forgotPasswordLink) {
    forgotPasswordLink.addEventListener('click', async (e) => {
        e.preventDefault();
        showForgotPasswordModal();
    });
}

function showForgotPasswordModal() {
    const i18n = new I18n();

    const modalHTML = `
        <div class="modal" id="forgotPasswordModal" style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 9999;">
            <div class="modal-content" style="background: var(--bg-primary); padding: 30px; max-width: 440px; width: 90%; border-radius: 16px; box-shadow: 0 8px 24px var(--shadow-md); position: relative;">
                <button class="close-button" id="closeForgotModal" style="position: absolute; top: 15px; right: 20px; background: none; border: none; font-size: 24px; cursor: pointer; color: var(--text-secondary);">✕</button>
                <h2 style="color: var(--text-primary); margin-bottom: 16px; font-size: 24px;">${i18n.t(
                    'resetPassword'
                )}</h2>
                <p style="color: var(--text-secondary); margin-bottom: 24px;">${i18n.t('resetPasswordMessage')}</p>
                
                <div class="form-group" style="text-align: left;">
                    <label for="resetEmail" style="display: block; margin-bottom: 8px; font-weight: 500; color: var(--text-primary);">${i18n.t(
                        'email'
                    )}</label>
                    <input type="email" id="resetEmail" placeholder="your@email.com" required style="width: 100%; padding: 12px 16px; border: 1px solid var(--border-color); border-radius: 8px; font-size: 14px; background: var(--bg-primary); color: var(--text-primary);">
                </div>
                
                <button id="sendResetBtn" class="btn btn-primary" style="width: 100%; margin-top: 20px; padding: 12px 24px; background: linear-gradient(135deg, var(--color-blue) 0%, #1557b0 100%); color: white; border: none; border-radius: 8px; font-weight: 600; cursor: pointer;">
                    ${i18n.t('sendResetLink')}
                </button>
            </div>
        </div>
    `;

    document.body.insertAdjacentHTML('beforeend', modalHTML);

    const modal = document.getElementById('forgotPasswordModal');
    const closeBtn = document.getElementById('closeForgotModal');
    const sendBtn = document.getElementById('sendResetBtn');
    const emailInput = document.getElementById('resetEmail');

    const api = new ApiClient();

    // Close modal
    closeBtn.addEventListener('click', () => modal.remove());
    modal.addEventListener('click', (e) => {
        if (e.target === modal) modal.remove();
    });

    // Send reset link
    sendBtn.addEventListener('click', async () => {
        const email = emailInput.value.trim();

        if (!email) {
            showMessage(i18n.t('pleaseEnterEmail'), 'error');
            return;
        }

        const originalText = sendBtn.textContent;

        try {
            sendBtn.disabled = true;
            sendBtn.textContent = i18n.t('sending');

            await api.forgotPassword(email);

            modal.remove();
            showMessage(i18n.t('resetLinkSent'), 'success');
        } catch (error) {
            console.error('Forgot password error:', error);
            showMessage(error.message || i18n.t('resetLinkFailed'), 'error');
        } finally {
            sendBtn.disabled = false;
            sendBtn.textContent = originalText;
        }
    });
}

function showMessage(message, type) {
    const messageEl =
        type === 'error' ? document.getElementById('error-message') : document.getElementById('success-message');

    messageEl.textContent = message;
    messageEl.style.display = 'flex';

    setTimeout(() => {
        messageEl.style.display = 'none';
    }, 5000);
}
