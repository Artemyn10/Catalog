document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('loginForm');
    const emailInput = document.getElementById('email');
    const passwordInput = document.getElementById('password');

    // Функция валидации email
    function validateEmail(email) {
        const re = new RegExp("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
        return re.test(email);
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
            } else if (this.value.length < 6) {
                showError(this, 'Пароль не менее 6 символов');
            }
        });
    }

    // Валидация на submit
    if (form) {
        form.addEventListener('submit', function (e) {
            let isValid = true;
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
            } else if (passwordInput.value.length < 6) {
                showError(passwordInput, 'Пароль не менее 6 символов');
                isValid = false;
            }

            if (!isValid) {
                e.preventDefault();  // Блокируем отправку
                return;
            }

            // AJAX-отправка
            e.preventDefault();
            const data = {
                email: emailInput.value.trim(),
                password: passwordInput.value
            };
            fetch("/api/account/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data)
            })
                .then(response => response.json())
                .then(result => {
                    const box = document.getElementById("loginResult");
                    if (result.success) {
                        box.innerHTML = `
                        <div class="alert alert-success text-center">
                            Успешный вход! Перенаправление...
                        </div>`;
                        form.reset();
                        setTimeout(() => {
                            window.location.href = "/";
                        }, 1200);
                    } else {
                        box.innerHTML = `
                        <div class="alert alert-danger text-center">
                            ${result.error || 'Ошибка входа. Попробуйте снова.'}
                        </div>`;
                    }
                })
                .catch(error => {
                    console.error('Ошибка:', error);
                    document.getElementById("loginResult").innerHTML = `
                    <div class="alert alert-danger text-center">
                        Произошла ошибка. Попробуйте снова.
                    </div>`;
                });
        });
    }
});