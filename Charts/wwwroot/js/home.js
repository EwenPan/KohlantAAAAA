const data = {
    labels: ["Nombres de votes"],
    url : "/chart",
    datasets: [{
        label: "Gaystap",
        data: [0],
        backgroundColor: [
            'rgba(250, 237, 39, 1)',
        ],
    }, {
        label: "Mc(do)2",
            data: [0,0],
            backgroundColor: [
                'rgba(237, 38, 38, 1)',
            ],
        }]
};

const ctx = document.getElementById('myChart').getContext('2d');
const myChart = new Chart(ctx, {
    type: 'bar',
    data: data,
    options: {
        animation: {
            duration: 0
        }
    }
});



let gurvanScore = parseInt(localStorage.getItem("gurvanScore"));
let nathanScore = parseInt(localStorage.getItem("nathanScore"));
let varez = parseInt(localStorage.getItem("nathanScore"));

const connection = new signalR.HubConnectionBuilder()
    .withUrl(data.url)
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log("err");
        console.log(err);
        setTimeout(start, 5000);
    }
}

connection.onclose(async () => {
    await start();
});


connection.on("clear", function () {
    gurvanScore = 0;
    myChart.data.datasets[0].data[0] = gurvanScore;
    localStorage.setItem('gurvanScore', gurvanScore);
    nathanScore = 0;
    myChart.data.datasets[1].data[0] = nathanScore;
    localStorage.setItem('nathanScore', nathanScore);
    myChart.update();
});

connection.on("addChartData", function (score) {
    if (score.label == "Gurvan") {
        gurvanScore = score.value;
        myChart.data.datasets[0].data[0] = gurvanScore;
        localStorage.setItem('gurvanScore', gurvanScore);

    }
    else if (score.label == "Nathan") {

        nathanScore = score.value;
        myChart.data.datasets[1].data[0] = nathanScore;
        localStorage.setItem('nathanScore', nathanScore);
    }

    myChart.update();
});

// Start the connection.
start().then(() => {});