function openAddMenu() {
    console.log("trigger");
    document.getElementById("birthdayModal").style.display = "block";
}

function closeAddMenu() {
    document.getElementById("birthdayModal").style.display = "none";
}

window.onclick = function (event) {
    let modal = document.getElementById("birthdayModal");
    if (event.target == modal) {
        modal.style.display = "none";
    }
}

function submitBirthdayForm() {
    var requestData = new FormData();
    requestData.append("fullName", document.getElementById("fullName").value);
    requestData.append("birthdayDate", document.getElementById("birthdayDate").value);

    var fileInput = document.getElementById("profileImage");
    var file = fileInput.files[0];
    requestData.append("file", file); 

    fetch("/api/Birthday/CreateUser", {
        method: 'POST',
        body: requestData 
    })
        .then(function (response) {
            if (!response.ok) {
                throw new Error('Произошла ошибка ' + response.status);
            }
            console.log('Запрос успешно отправлен');
            return response.json(); 
        })
        .then(data => {
            if (data.result) {
                fetchItems();
                closeAddMenu();
            }
            console.log("Ответ от сервера: " + data.comment);
        })
        .catch(function (error) {
            console.error('Произошла ошибка при отправке запроса:', error);
        });
}
