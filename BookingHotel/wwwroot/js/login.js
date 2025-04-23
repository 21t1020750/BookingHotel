// Toggle between login and register forms
document.getElementById('loginTab').addEventListener('click', function () {
    this.classList.add('text-blue-600', 'border-blue-600');
    document.getElementById('registerTab').classList.remove('text-blue-600', 'border-blue-600');
    document.getElementById('loginForm').classList.remove('hidden');
    document.getElementById('registerForm').classList.add('hidden');
});

document.getElementById('registerTab').addEventListener('click', function () {
    this.classList.add('text-blue-600', 'border-blue-600');
    document.getElementById('loginTab').classList.remove('text-blue-600', 'border-blue-600');
    document.getElementById('registerForm').classList.remove('hidden');
    document.getElementById('loginForm').classList.add('hidden');
});

// Toggle password visibility
function togglePassword(id) {
    const input = document.getElementById(id);
    const icon = input.nextElementSibling.querySelector('i');
    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.remove('fa-eye');
        icon.classList.add('fa-eye-slash');
    } else {
        input.type = 'password';
        icon.classList.remove('fa-eye-slash');
        icon.classList.add('fa-eye');
    }
}

// Client-side validation for login form
document.getElementById('loginForm').addEventListener('submit', function (e) {
    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;

    let valid = true;
    if (!email || !email.includes('@')) {
        document.getElementById('loginEmailError').classList.remove('hidden');
        valid = false;
    } else {
        document.getElementById('loginEmailError').classList.add('hidden');
    }

    if (!password) {
        document.getElementById('loginPasswordError').classList.remove('hidden');
        valid = false;
    } else {
        document.getElementById('loginPasswordError').classList.add('hidden');
    }

    if (!valid) {
        e.preventDefault();
    }
});

// Client-side validation for register form
document.getElementById('registerForm').addEventListener('submit', function (e) {
    const name = document.getElementById('registerName').value;
    const email = document.getElementById('registerEmail').value;
    const phone = document.getElementById('registerPhone').value;
    const password = document.getElementById('registerPassword').value;
    const confirmPassword = document.getElementById('registerConfirmPassword').value;

    let valid = true;

    if (!name) {
        document.getElementById('registerNameError').classList.remove('hidden');
        valid = false;
    } else {
        document.getElementById('registerNameError').classList.add('hidden');
    }

    if (!email || !email.includes('@')) {
        document.getElementById('registerEmailError').classList.remove('hidden');
        valid = false;
    } else {
        document.getElementById('registerEmailError').classList.add('hidden');
    }

    if (!phone) {
        document.getElementById('registerPhoneError').classList.remove('hidden');
        valid = false;
    } else {
        document.getElementById('registerPhoneError').classList.add('hidden');
    }

    if (!password || password.length < 6) {
        document.getElementById('registerPasswordError').classList.remove('hidden');
        valid = false;
    } else {
        document.getElementById('registerPasswordError').classList.add('hidden');
    }

    if (password !== confirmPassword) {
        document.getElementById('registerConfirmPasswordError').classList.remove('hidden');
        valid = false;
    } else {
        document.getElementById('registerConfirmPasswordError').classList.add('hidden');
    }

    if (!valid) {
        e.preventDefault();
    }
});