const ProductImageManager = {
    selectors: {
        fileInput: 'input[name="files"]',
        previewContainer: '#previewContainer'
    },
    init() {
        this.$preview = $(this.selectors.previewContainer);
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

        validImages.forEach(file => {
            const imgUrl = URL.createObjectURL(file);
            const $imgElement = this.renderImage(imgUrl, file.name);
            this.$preview.append($imgElement);
        });
    },
    renderImage(url, fileName) {
        const $img = $('<img>', {
            src: url,
            title: fileName,
            css: {
                width: '120px',
                height: '120px',
                objectFit: 'cover',
                borderRadius: '8px',
                boxShadow: '0 2px 6px rgba(0,0,0,0.15)',
                margin: '8px',
                transition: 'transform 0.2s'
            }
        });
        $img.on('load', function () {
            URL.revokeObjectURL(url);
        });
        return $img;
    }
};
$(() => ProductImageManager.init());
