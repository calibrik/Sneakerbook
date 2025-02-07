function onImageLoad()
{
    document.getElementById("image").style.display = "block";
    document.getElementById("spinner").style.display = "none";
}

function onImageLoadById(id)
{
    document.getElementById("image "+id).style.display = "block";
    document.getElementById("spinner "+id).style.display = "none";
}
function DisableOnChange(inputId,checkboxId)
{
    document.getElementById(inputId).disabled = !document.getElementById(checkboxId).checked;
}
function onConfirmPasswordChange()
{
    
}

function kickMember(userId,clubId)
{
    fetch('/api/ClubApi/KickMember',
        {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({
                userId: userId,
                clubId: clubId,
            }),
        })
        .then(response => console.log(response))
        .then(()=>window.location.reload())
        .catch(error => console.log(error));
}