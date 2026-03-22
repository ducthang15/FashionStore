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