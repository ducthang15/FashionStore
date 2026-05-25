window.addEventListener("scroll", function () {
    const nav = document.querySelector(".navbar");
    if (window.scrollY > 80) {
        nav.classList.add("scrolled");
    } else {
        nav.classList.remove("scrolled");
    }
});

const btn = document.getElementById("backToTop");

if (btn) {
    document.addEventListener("scroll", () => {
        const scroll = document.documentElement.scrollTop;

        btn.style.display = scroll > 300 ? "block" : "none";
    });

    btn.addEventListener("click", (e) => {
        e.preventDefault();
        document.documentElement.scrollTo({
            top: 0,
            behavior: "smooth"
        });
    });
}

$('#checkAll').on('change', function () {
    $('input[name="ids"]').prop('checked', $(this).prop('checked'));
});
$(document).on('click', '#btnDeleteSelect', function (e) {

    e.preventDefault();

    let checked = $('input[name="ids"]:checked').length;

    if (checked === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Chưa chọn sản phẩm',
            text: 'Vui lòng chọn ít nhất 1 sản phẩm!'
        });
        return;
    }

    Swal.fire({
        title: 'Xác nhận xoá?',
        text: `Bạn sắp xoá ${checked} sản phẩm!`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Xoá ngay',
        cancelButtonText: 'Huỷ',
        confirmButtonColor: '#d33'
    }).then((result) => {
        if (result.isConfirmed) {
            $(this).closest('form').trigger('submit');
        }
    });
});
$(document).on('click', '#btnDeleteAll', function () {

    Swal.fire({
        title: 'Xoá toàn bộ?',
        text: 'Hành động này không thể hoàn tác!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Xoá tất cả',
        cancelButtonText: 'Huỷ',
        confirmButtonColor: '#d33'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = "/Admin/ProductAdmin/DeleteAll";
        }
    });
});
$(function () {
    const $mainImg = $("#mainImage");
    const $modalImg = $("#modalImage");
    const $modalEle = $('#imageModal');
    let scale = 1;
    $mainImg.on("click", function () {
        const currentSrc = $(this).attr("src");

        $modalImg.attr("src", currentSrc).css({
            "transform": "scale(1)",
            "transform-origin": "center center"
        });

        scale = 1;
        $modalEle.modal('show');
    });
    $modalImg.on("wheel", function (e) {
        e.preventDefault();
        const oe = e.originalEvent;
        const rect = this.getBoundingClientRect();
        const offsetX = oe.clientX - rect.left;
        const offsetY = oe.clientY - rect.top;
        const x = (offsetX / rect.width) * 100;
        const y = (offsetY / rect.height) * 100;
        scale += oe.deltaY < 0 ? 0.25 : -0.25;
        scale = Math.max(1, Math.min(scale, 5));

        $(this).css({
            "transform-origin": `${x}% ${y}%`,
            "transform": `scale(${scale})`,
            "cursor": scale > 1 ? "zoom-out" : "zoom-in"
        });
    });
    $modalEle.on('hidden.bs.modal', function () {
        scale = 1;
        $modalImg.css("transform", "scale(1)");
    });
});
$(function () {

    $('.live-search-input').each(function () {

        let searchTimeout;

        const $searchInput = $(this);
        const $searchBox = $searchInput.closest('.live-search-box');
        const $searchResults = $searchBox.find('.search-results-dropdown');

        $searchInput.on('input', function () {

            clearTimeout(searchTimeout);

            const query = $(this).val().trim();

            // Ít hơn 2 ký tự thì ẩn
            if (query.length < 2) {
                $searchResults.hide();
                return;
            }

            searchTimeout = setTimeout(function () {

                $searchResults
                    .html(`
                        <div class="p-3 text-center text-white">
                            <i class="fa-solid fa-spinner fa-spin"></i>
                            Searching...
                        </div>
                    `)
                    .show();

                $.ajax({
                    url: '/Search/LiveSearch',
                    type: 'GET',
                    data: {
                        keyword: query
                    },
                    success: function (data) {

                        $searchResults.empty();

                        if (data.length > 0) {

                            $.each(data, function (index, item) {

                                $searchResults.append(`
                                    <a href="${item.url}" class="search-item">
                                        ${item.image
                                        ? `<img src="${item.image}" alt="${item.title}">`
                                        : ''
                                    }

                                        <div class="search-item-info">
                                            <span class="search-item-title">
                                                ${item.title}
                                            </span>

                                            <span class="search-item-meta">
                                               ${item.type}
                                            </span>
                                        </div>
                                    </a>
                                `);

                            });

                        } else {

                            $searchResults.html(`
                                <div class="p-3 text-center text-white-50"
                                     style="font-size:13px;">
                                    No matching results found.
                                </div>
                            `);

                        }
                    },
                    error: function (xhr, status, error) {
                        console.error('Search Error:', error);
                    }
                });

            }, 300);

        });

        // Click ngoài thì ẩn
        $(document).on('click', function (e) {

            if (!$searchBox.is(e.target) &&
                $searchBox.has(e.target).length === 0) {

                $searchResults.hide();
            }

        });

    });

});
let lastScrollTop = 0;
const bottomNav = document.querySelector(".ios-bottom-nav");
// thanh bottom
window.addEventListener("scroll", function () {

    let currentScroll =
        window.pageYOffset || document.documentElement.scrollTop;

    // Vuốt xuống -> ẩn
    if (currentScroll > lastScrollTop && currentScroll > 100) {

        bottomNav.classList.add("hide-nav");

    }
    // Vuốt lên -> hiện
    else {

        bottomNav.classList.remove("hide-nav");

    }

    lastScrollTop = currentScroll <= 0 ? 0 : currentScroll;
});