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
    let button=document.getElementById("kickButton"+userId);
    let origText=button.innerHTML;
    button.innerHTML=`
<div class="spinner-border" role="status">
  <span class="visually-hidden">Loading...</span>
</div>`;
    fetch('/api/ClubApi/KickMember',
        {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({
                userId: userId,
                clubId: clubId,
            }),
        })
        .then(async response => {
            let data =await response.json();
            console.log(response);
            if (!response.ok) {
                button.innerHTML=origText;
                showError(data.message);
                setTimeout(() => { location.reload(); }, 3000);
            }
            else {
                location.reload();
            }
        });
}
function showError(message) {
    document.getElementById("errorMessage").innerText = message;
    let toast = new bootstrap.Toast(document.getElementById("errorToast"));
    toast.show();
}
function leaveCLub(id)
{
    let button=document.getElementById("leaveButton");
    let origText=button.innerHTML;
    button.innerHTML=`
<div class="spinner-border" role="status">
  <span class="visually-hidden">Loading...</span>
</div>`;
    fetch('/api/ClubApi/Leave',
        {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(id)
        })
        .then(async response => {
            let data = await response.json();
            console.log(response);
            if (!response.ok) {
                button.innerHTML=origText;
                showError(data.message);
                setTimeout(() => { location.reload(); }, 3000);
            }
            else {
                location.reload();
            }
        })
}
function joinClub(id)
{
    let button=document.getElementById("joinButton");
    let origText=button.innerHTML;
    button.innerHTML=`
<div class="spinner-border" role="status">
  <span class="visually-hidden">Loading...</span>
</div>`;
    fetch('/api/ClubApi/Join',
        {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(id)
        })
        .then(async response => {
            let data = await response.json();
            console.log(response);
            if (!response.ok) {
                button.innerHTML=origText;
                showError(data.message);
                setTimeout(() => { location.reload(); }, 3000);
            }
            else {
                location.reload();
            }
        })
}