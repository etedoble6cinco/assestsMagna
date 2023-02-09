var PreviewPurchaseReceipt = function (id) {
    $("#btnPreviewPurchaseReceipt").val("Working on it...");
    $("#btnPreviewPurchaseReceipt").attr("disabled", "disabled");

    $.ajax({
        type: "GET",
        url: "/Asset/DownloadPurchaseReceipt?id=" + id,
        success: function (result) {
            var _base64ToArrayBuffer = base64ToArrayBuffer(result.DocByte);
            // saveByteArray(result.FileName, _base64ToArrayBuffer, result.ContentType);
            // The workerSrc property shall be specified.
            pdfjsLib.GlobalWorkerOptions.workerSrc =
                '../lib/pdfjs/pdf.worker.min.js';
            var pdfData = atob(result.DocByte);
            // Using DocumentInitParameters object to load binary data.
            var loadingTask = pdfjsLib.getDocument({ data: pdfData });
            loadingTask.promise.then(function (pdf) {
                console.log('PDF loaded');

                // Fetch the first page
                var pageNumber = 1;
                pdf.getPage(pageNumber).then(function (page) {
                    console.log('Page loaded');

                    var scale = 1.5;
                    var viewport = page.getViewport({ scale: scale });

                    // Prepare canvas using PDF page dimensions
                    var canvas = document.getElementById('PdfPurchaseReceipt');
                    var context = canvas.getContext('2d');
                    canvas.height = viewport.height;
                    canvas.width = viewport.width;

                    // Render PDF page into canvas context
                    var renderContext = {
                        canvasContext: context,
                        viewport: viewport
                    };
                    var renderTask = page.render(renderContext);
                    renderTask.promise.then(function () {

                    });
                });
            }, function (reason) {
                // PDF loading error
                console.error(reason);
            });

        }
    });
}