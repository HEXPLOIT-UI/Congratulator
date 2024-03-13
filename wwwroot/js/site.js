document.addEventListener("DOMContentLoaded", function () {
    fetchItems();
});

function fetchItems() {
    fetch('/api/Birthday/GetUpcomingBirthdays')
        .then(response => response.json())
        .then(data => displayItems(data))
        .catch(error => console.error('Ошибка загрузки данных: ', error));
}

function displayItems(items) {
    const tableBody = document.getElementById('sortTable').querySelector('tbody');
    tableBody.innerHTML = ''; // Очистить текущие строки
    items.forEach(item => {
        const row = tableBody.insertRow();
        row.setAttribute('data-id', item.id);
        const cellPhoto = row.insertCell(0);
        var img = document.createElement("img");
        img.src = "/images/" + item.profileImageUri;
        img.style.width = "100px"; 
        img.style.height = "auto";
        cellPhoto.appendChild(img);
        const cellName = row.insertCell(1);
        cellName.textContent = item.fullName;
        const cellDate = row.insertCell(2);
        cellDate.textContent = item.birthdayDate.split('T')[0];
    });
}

function sortTable(column) {
    var table, rows, switching, i, x, y, shouldSwitch, dir, switchcount = 0;
    table = document.getElementById("sortTable");
    switching = true;
    dir = "asc";
    while (switching) {
        switching = false;
        rows = table.rows;
        for (i = 1; i < (rows.length - 1); i++) {
            shouldSwitch = false;
            x = rows[i].getElementsByTagName("TD")[column];
            y = rows[i + 1].getElementsByTagName("TD")[column];
            if (dir == "asc") {
                if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
                    shouldSwitch = true;
                    break;
                }
            } else if (dir == "desc") {
                if (x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
                    shouldSwitch = true;
                    break;
                }
            }
        }
        if (shouldSwitch) {
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
            switchcount++;
        } else {
            if (switchcount == 0 && dir == "asc") {
                dir = "desc";
                switching = true;
            }
        }
    }
}

document.getElementById('editButton').addEventListener('click', function () {
    let cells = document.querySelectorAll('td');
    cells.forEach(cell => {
        if (!cell.querySelector('button')) {
            cell.contentEditable = 'true';
        }
    });

    document.querySelectorAll('tr').forEach(row => {
        let deleteCell = row.insertCell(-1);
        deleteCell.innerHTML = '<button class="deleteBtn">&#10006;</button>';
        deleteCell.querySelector('.deleteBtn').onclick = function () {
            submitDeleteRequest(row);
        };

        let saveCell = row.insertCell(-1);
        saveCell.innerHTML = '<button class="saveBtn">&#10004;</button>';
        saveCell.querySelector('.saveBtn').onclick = function () {
            let cells = row.querySelectorAll('td');
            let data = {};
            data.id = row.getAttribute('data-id');
            data.fullName = cells[1].innerText;
            data.birthdayDate = cells[2].innerText;
            // здесь код для отправки данных на сервер
            console.log("отправлен запрос на изменение след.данных: id: " + data.id + " name: " + data.fullName + " date: " + data.birthdayDate);
            var params = new URLSearchParams(data).toString();
            var url = '/api/Birthday/UpdateUser?' + params;
            fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error('Произошла ошибка ' + response.status);
                    }
                    console.log('Запрос успешно отправлен');

                    return response.json();
                })
                .then(data => {
                    console.log("Ответ от сервера: " + data.result + " " + data.comment);
                })
                .catch(function (error) {
                    console.error('Произошла ошибка при отправке запроса:', error);
                });
        };
    });

    document.getElementById('saveButton').style.display = 'block';
    this.style.display = 'none';
});

document.getElementById('saveButton').addEventListener('click', function () {
    let rows = document.querySelectorAll('tr');

    rows.forEach(row => {
        let cells = row.querySelectorAll('td');
        cells.forEach(cell => {
            cell.contentEditable = 'false';
        });
        row.deleteCell(-1);
        row.deleteCell(-1);
    });

    document.getElementById('editButton').style.display = 'block';
    this.style.display = 'none';
});

function submitDeleteRequest(row) {
    var requestData = new FormData();
    requestData.append("id", row.getAttribute('data-id'));

    fetch("/api/Birthday/DeleteUser", {
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
                row.remove();
            }
            console.log("Ответ от сервера: " + data.result + " " + data.comment);
        })
        .catch(function (error) {
            console.error('Произошла ошибка при отправке запроса:', error);
        });
}

var showType = false;
function displayAllBirthdays() {
    var switchButton = document.getElementById("switchBirthdayShowType");
    var switchText = document.getElementById("showTypeText");
    if (!showType) {
        fetch('/api/Birthday/GetAllBirthdays')
            .then(response => response.json())
            .then(data => {
                displayItems(data);
                switchButton.innerText = "Показать ближайшие дни рождения";
                switchText.innerText = "Список всех дней рождений"
                showType = true;
            })
            .catch(error => console.error('Ошибка загрузки данных: ', error));
    } else {
        fetchItems();
        switchButton.innerText = "Показать все дни рождения";
        switchText.innerText = "Список ближайших дней рождения";
        showType = false;
    }
}