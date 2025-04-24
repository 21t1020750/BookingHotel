const slider = document.getElementById('slider');
const images = slider.getElementsByTagName('img');
const pagination = document.querySelector('.pagination');
let currentIndex = 0;
const totalImages = images.length;

// Tạo các nút dot tương ứng với số lượng ảnh
for (let i = 0; i < totalImages; i++) {
    const dot = document.createElement('div');
    dot.classList.add('dot');
    if (i === 0) dot.classList.add('active'); // Đánh dấu dot đầu tiên là active
    dot.addEventListener('click', () => {
        goToSlide(i);
    });
    pagination.appendChild(dot);
}

const dots = document.querySelectorAll('.dot');

// Hàm chuyển đến slide cụ thể
function goToSlide(index) {
    currentIndex = index;
    slider.style.transform = `translateX(-${currentIndex * 100}%)`;
    updateDots();
}

// Hàm cập nhật trạng thái nút dot
function updateDots() {
    dots.forEach((dot, index) => {
        dot.classList.toggle('active', index === currentIndex);
    });
}

// Tự động chuyển ảnh mỗi 3 giây
function autoSlide() {
    currentIndex = (currentIndex + 1) % totalImages;
    goToSlide(currentIndex);
}

// Bắt đầu tự động chuyển ảnh
setInterval(autoSlide, 5000);
