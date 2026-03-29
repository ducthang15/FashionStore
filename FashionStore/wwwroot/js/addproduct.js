const ProductImageManager = {
    selectors: {
        fileInput: 'input[name="files"]',
        previewContainer: '#previewContainer',
        mainInput: '#mainImageIndex'
    },

    init() {
        this.$preview = $(this.selectors.previewContainer);
        this.$mainInput = $(this.selectors.mainInput);
        if (!this.$preview.length) return;

        this.bindEvents();
    },

    bindEvents() {
        $(document).on('change', this.selectors.fileInput, (e) => this.handleFileChange(e));
    },

    handleFileChange(e) {
        this.$preview.empty();

        const files = Array.from(e.target.files);
        const validImages = files.filter(file => file.type.startsWith('image/'));

        validImages.forEach((file, index) => {
            const imgUrl = URL.createObjectURL(file);
            const $imgElement = this.renderImage(imgUrl, file.name, index);
            this.$preview.append($imgElement);
        });
    },

    renderImage(url, fileName, index) {
        const $wrapper = $('<div>', {
            css: {
                position: 'relative',
                display: 'inline-block'
            }
        });

        const $img = $('<img>', {
            src: url,
            title: fileName,
            css: {
                width: '120px',
                height: '120px',
                objectFit: 'cover',
                borderRadius: '8px',
                margin: '8px',
                cursor: 'pointer',
                border: '3px solid transparent'
            }
        });

        const $radio = $('<input>', {
            type: 'radio',
            name: 'mainImageSelect',
            value: index,
            css: {
                position: 'absolute',
                top: '5px',
                left: '5px'
            }
        });
        if (index === 0) {
            $radio.prop('checked', true);
            this.$mainInput.val(0);
            $img.css('border', '3px solid red');
        }

        $radio.on('change', () => {
            $('input[name="mainImageSelect"]').each(function () {
                $(this).siblings('img').css('border', '3px solid transparent');
            });

            $img.css('border', '3px solid red');
            $('#mainImageIndex').val(index);
        });

        $img.on('load', () => URL.revokeObjectURL(url));

        $wrapper.append($img, $radio);
        return $wrapper;
    }
};
$(() => ProductImageManager.init());

const SummernoteManager = {
    selectors: {
        editor: '#Description',
        uploadUrl: '/Admin/Upload/UploadImage'
    },

    init() {
        const $editor = $(this.selectors.editor);
        if (!$editor.length) return; 

        this.initEditor($editor);
    },

    initEditor($el) {
        $el.summernote({
            height: 400,
            placeholder: 'Nhập mô tả sản phẩm...',
            lang: 'vi-VN', // Nếu bạn có file ngôn ngữ tiếng Việt
            toolbar: [
                ['style', ['style']],
                ['font', ['bold', 'underline', 'clear']],
                ['color', ['color']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['insert', ['link', 'picture', 'video']],
                ['view', ['fullscreen', 'codeview', 'help']]
            ],
            callbacks: {

                onImageUpload: (files) => {
                    Array.from(files).forEach(file => this.uploadImage($el, file));
                }
            }
        });
    },

    uploadImage($el, file) {
        const data = new FormData();
        data.append("file", file);

        $.ajax({
            url: this.selectors.uploadUrl,
            method: 'POST',
            data: data,
            contentType: false,
            processData: false,
            success: (url) => {
                $el.summernote('insertImage', url);
            },
            error: () => {
                alert('Upload ảnh thất bại. Vui lòng kiểm tra lại định dạng hoặc kích thước file.');
            }
        });
    }
};

// Khởi chạy khi DOM đã sẵn sàng
$(() => SummernoteManager.init());