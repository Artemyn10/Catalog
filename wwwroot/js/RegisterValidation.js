document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('registerForm');
    const nameInput = document.getElementById('name');
    const emailInput = document.getElementById('email');
    const passwordInput = document.getElementById('password');

    // Функция валидации email
    function validateEmail(email) {
        const re = new RegExp("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
        return re.test(email);
    }

    // Функция валидации пароля (6+, заглавная, строчная, цифра)
    function validatePassword(password) {
        if (password.length < 6) return false;
        const upperCase = /[A-Z]/.test(password);
        const lowerCase = /[a-z]/.test(password);
        const digit = /[0-9]/.test(password);
        return upperCase && lowerCase && digit;
    }

    // Функция показа ошибки
    function showError(input, message) {
        const errorDiv = input.nextElementSibling;
        errorDiv.textContent = message;
        input.classList.add('is-invalid');
    }

    // Функция очистки ошибки
    function clearError(input) {
        const errorDiv = input.nextElementSibling;
        errorDiv.textContent = '';
        input.classList.remove('is-invalid');
    }

    // Валидация на blur (немедленная обратная связь)
    if (nameInput) {
        nameInput.addEventListener('blur', function () {
            clearError(this);
            if (!this.value.trim()) {
                showError(this, 'Имя обязательно');
            } else if (this.value.trim().length < 2 || this.value.trim().length > 50) {
                showError(this, 'Имя должно быть от 2 до 50 символов');
            }
        });
    }

    if (emailInput) {
        emailInput.addEventListener('blur', function () {
            clearError(this);
            if (!this.value.trim()) {
                showError(this, 'Email обязателен');
            } else if (!validateEmail(this.value)) {
                showError(this, 'Некорректный формат email');
            }
        });
    }

    if (passwordInput) {
        passwordInput.addEventListener('blur', function () {
            clearError(this);
            if (!this.value.trim()) {
                showError(this, 'Пароль обязателен');
            } else if (!validatePassword(this.value)) {
                showError(this, 'Пароль должен быть не менее 6 символов, содержать заглавную букву, строчную и цифру');
            }
        });
    }

    // Валидация на submit
    if (form) {
        form.addEventListener('submit', function (e) {
            let isValid = true;
            // Проверка name
            if (!nameInput.value.trim()) {
                showError(nameInput, 'Имя обязательно');
                isValid = false;
            } else if (nameInput.value.trim().length < 2 || nameInput.value.trim().length > 50) {
                showError(nameInput, 'Имя должно быть от 2 до 50 символов');
                isValid = false;
            }
            // Проверка email
            if (!emailInput.value.trim()) {
                showError(emailInput, 'Email обязателен');
                isValid = false;
            } else if (!validateEmail(emailInput.value)) {
                showError(emailInput, 'Некорректный формат email');
                isValid = false;
            }
            // Проверка password
            if (!passwordInput.value.trim()) {
                showError(passwordInput, 'Пароль обязателен');
                isValid = false;
            } else if (!validatePassword(passwordInput.value)) {
                showError(passwordInput, 'Пароль должен быть не менее 6 символов, содержать заглавную букву, строчную и цифру');
                isValid = false;
            }

            if (!isValid) {
                e.preventDefault();  // Блокируем отправку
                return;
            }

            // AJAX-отправка (аналогично Login)
            e.preventDefault();
            const data = {
                name: nameInput.value.trim(),
                email: emailInput.value.trim(),
                password: passwordInput.value
            };
            fetch("/api/account/register", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data)
            })
                .then(response => response.json())
                .then(result => {
                    const box = document.getElementById("registerResult");
                    if (result.success) {
                        box.innerHTML = `
                        <div class="alert alert-success text-center">
                            Успешная регистрация! Перенаправление на вход...
                        </div>`;
                        form.reset();
                        setTimeout(() => {
                            window.location.href = "/Account/Login";
                        }, 1200);
                    } else {
                        box.innerHTML = `
                        <div class="alert alert-danger text-center">
                            ${result.error || 'Ошибка регистрации. Попробуйте снова.'}
                        </div>`;
                    }
                })
                .catch(error => {
                    console.error('Ошибка:', error);
                    document.getElementById("registerResult").innerHTML = `
                    <div class="alert alert-danger text-center">
                        Произошла ошибка. Попробуйте снова.
                    </div>`;
                });
        });
    }
});