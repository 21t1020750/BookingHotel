// Toggle between login and register forms
const loginTab = document.getElementById('loginTab');
const registerTab = document.getElementById('registerTab');
const loginForm = document.getElementById('loginForm');
const registerForm = document.getElementById('registerForm');

// Debug: Check if elements are found
console.log('loginTab:', loginTab);
console.log('registerTab:', registerTab);
console.log('loginForm:', loginForm);
console.log('registerForm:', registerForm);

// Function to activate a tab
function activateTab(tab) {
    console.log('Activating tab:', tab);
    if (tab === 'login') {
        loginTab.classList.add('text-blue-600', 'border-b-2', 'border-blue-600');
        loginTab.classList.remove('text-gray-500');
        registerTab.classList.remove('text-blue-600', 'border-b-2', 'border-blue-600');
        registerTab.classList.add('text-gray-500');
        loginForm.classList.remove('hidden');
        registerForm.classList.add('hidden');
        console.log('Login tab activated');
    } else {
        registerTab.classList.add('text-blue-600', 'border-b-2', 'border-blue-600');
        registerTab.classList.remove('text-gray-500');
        loginTab.classList.remove('text-blue-600', 'border-b-2', 'border-blue-600');
        loginTab.classList.add('text-gray-500');
        registerForm.classList.remove('hidden');
        loginForm.classList.add('hidden');
        console.log('Register tab activated');
    }
}

// Check URL query parameter to activate the correct tab on page load
document.addEventListener('DOMContentLoaded', () => {
    const urlParams = new URLSearchParams(window.location.search);
    const tab = urlParams.get('tab');
    console.log('Query parameter tab:', tab);
    if (tab === 'register') {
        activateTab('register');
    } else {
        activateTab('login'); // Default to login tab
    }
});

// Event listeners for tab clicks
loginTab.addEventListener('click', () => {
    activateTab('login');
    // Update URL without reloading
    history.pushState(null, '', '?tab=login');
});

registerTab.addEventListener('click', () => {
    activateTab('register');
    // Update URL without reloading
    history.pushState(null, '', '?tab=register');
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
loginForm.addEventListener('submit', function (e) {
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
registerForm.addEventListener('submit', function (e) {
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