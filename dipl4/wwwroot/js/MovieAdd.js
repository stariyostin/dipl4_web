const photoUploader = document.getElementById('photo-uploader');

photoUploader.addEventListener('change', (event) => {
    const file = photoUploader.files[0];
    var fileExtension = ['jpg', 'jpeg', 'png'];
    var ext = file.name.split('.').pop().toLowerCase();
    var isPhoto = false;
    fileExtension.forEach(function (elem) {
        if (elem == ext) {
            isPhoto = true;
            console.log(isPhoto);
        }
        console.log(isPhoto);
    }
    )
    if (isPhoto === false) {
        photoUploader.value = "";
        const movieImage = document.getElementById('uploadingMovieImage');
        movieImage.src = "";
    }
    else {
        const movieImage = document.getElementById('uploadingMovieImage');
        var fileReader = new FileReader();
        fileReader.onload = function () {
            movieImage.src = fileReader.result;
        }
        fileReader.readAsDataURL(photoUploader.files[0]);
    }
});