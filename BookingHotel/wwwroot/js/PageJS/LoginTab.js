function switchTab(tab) {
    window.location.href = `/Account/Login?tab=${tab}`;
}

// Hiển thị tab mặc định khi trang tải
document.addEventListener('DOMContentLoaded', function () {
    const tab = document.getElementById('tab-data').getAttribute('data-tab');
    const loginTab = document.getElementById('login');
    const registerTab = document.getElementById('register');

    if (tab === 'login') {
        loginTab.classList.remove('hidden');
        registerTab.classList.add('hidden');
    } else {
        loginTab.classList.add('hidden');
        registerTab.classList.remove('hidden');
    }
});