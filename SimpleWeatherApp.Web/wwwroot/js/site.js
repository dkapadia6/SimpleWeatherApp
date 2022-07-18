$(function () {
    $("#btnGetForecast").click(function (e) {
        e.preventDefault();

        var city = $("#city").val();
        var country = $("#country").val();

        if (CheckNullUndefined(city) || CheckNullUndefined(country)) {
            alert("Validation error: Please ensure both city and country fields are filled in");
        }
        else {
            /*$.get("https://localhost:5001/api/weather",
                {
                    city: city,
                    country: country
                }, function (data, status) {
                    $("#divResult").html("Weather result: " + data.message);
                }).fail(function(data){
                    alert(data.message);
                });*/
            var url = "https://localhost:5001/api/weather";

            $.ajax({
                type: "GET",
                url: url,
                data: { city: city, country: country},
            })
            .done(function(data, textStatus, jqXHR){
                if(!CheckNullUndefined(data.message)){
                    $('#divResult').html("Today's weather is : " + data.message);
                    $('#divResult').css("color", "blue");
                }
            })
            .fail(function(jqXHR, textStatus, errorThrown){
                //alert('Response: ' + jqXHR.responseText + '\nStatus: ' + jqXHR.status + '\nStatus text: ' + jqXHR.statusText);
                if(jqXHR.status == 429){ //Too many requests
                    $('#divResult').html("Unable to retrieve weather report as hourly limit of API consumption (5 times) has been exceeded. <br/>Please try again later.");
                    $('#divResult').css("color", "red");
                }

                if(jqXHR.status == 400){ //Bad request
                    $('#divResult').html("Unable to retrieve weather report due to a bad request.");
                    $('#divResult').css("color", "red");
                }
            });
        }
    });
});

function CheckNullUndefined(value) {
    return typeof value == 'string' && !value.trim() || typeof value == 'undefined' || value === null;
}