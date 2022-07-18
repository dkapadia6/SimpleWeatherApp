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
                beforeSend: function(xhr){
                    xhr.setRequestHeader('x-api-key', '+oD4DRdFy0ynKJCQ8A8TKQ==');
                }
            })
            .done(function(data, textStatus, jqXHR){
                if(!CheckNullUndefined(data.message)){
                    $('#divResult').html("Today's weather is : " + data.message);
                    $('#divResult').css("color", "blue");
                }
            })
            .fail(function(jqXHR, textStatus, errorThrown){
                $('#divResult').html("Response code: " + jqXHR.status + "<br/>" + jqXHR.responseText);
                $('#divResult').css("color", "red");
            });
        }
    });
});

function CheckNullUndefined(value) {
    return typeof value == 'string' && !value.trim() || typeof value == 'undefined' || value === null;
}