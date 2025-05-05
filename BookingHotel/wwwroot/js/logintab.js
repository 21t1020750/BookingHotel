function showTab(tabName) {
    const loginTab = document.getElementById('loginTab');
    const registerTab = document.getElementById('registerTab');
    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');

    if (tabName === 'login') {
        loginTab.classList.add('text-blue-600', 'border-b-2', 'border-blue-600');
        loginTab.classList.remove('text-gray-500', 'hover:text-blue-600');
        registerTab.classList.add('text-gray-500', 'hover:text-blue-600');
        registerTab.classList.remove('text-blue-600', 'border-b-2', 'border-blue-600');

        loginForm.classList.remove('hidden');
        registerForm.classList.add('hidden');
    } else {
        registerTab.classList.add('text-blue-600', 'border-b-2', 'border-blue-600');
        registerTab.classList.remove('text-gray-500', 'hover:text-blue-600');
        loginTab.classList.add('text-gray-500', 'hover:text-blue-600');
        loginTab.classList.remove('text-blue-600', 'border-b-2', 'border-blue-600');

        registerForm.classList.remove('hidden');
        loginForm.classList.add('hidden');
    }
}

// Khi trang tải, lấy giá trị tab từ thuộc tính data-tab và hiển thị tab tương ứng
document.addEventListener('DOMContentLoaded', function () {
    const tabElement = document.getElementById('tab-data');
    const tab = tabElement ? tabElement.getAttribute('data-tab') : 'login'; // Mặc định là login nếu không tìm thấy
    showTab(tab);
});

// Thêm sự kiện click cho các tab
document.getElementById('loginTab').addEventListener('click', function () {
    showTab('login');
});

document.getElementById('registerTab').addEventListener('click', function () {
    showTab('register');
});

// Hàm toggle hiển thị mật khẩu
function togglePassword(inputId) {
    const input = document.getElementById(inputId);
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