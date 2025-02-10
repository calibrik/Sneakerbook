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
async function performPostFromButton(action, controller, body, buttonId)
{
    let button=document.getElementById(buttonId);
    button.style.pointerEvents = "none";
    let origText=button.innerHTML;
    button.innerHTML=`
<div class="spinner-border" role="status">
  <span class="visually-hidden">Loading...</span>
</div>`;
    try {
        let response=await fetch(`/api/${controller}/${action}`,
            {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: body,
            });
            let data =await response.json();
            console.log(response);
            button.style.pointerEvents = "all";
            button.innerHTML=origText;
            if (!response.ok) {
                showError(data.message);
                // setTimeout(() => { location.reload(); }, 3000);
            }
            // else {
            //     location.reload();
            // }
        return {response,data};
        }
    catch(error) {
        console.log(error);
    }
}
function deleteClub(clubId)
{
    performPostFromButton("Delete","ClubApi",JSON.stringify(clubId),`deleteButton`)
        .then(({response,data})=>{
            if (!response.ok) {
                return;
            }
            document.getElementById("editButtonDiv").innerHTML=``;
            document.getElementById("removeOnDeleteDiv").innerHTML=``;
            document.getElementById("card").innerHTML=`<span class="text-center text-success"><strong>${data.message}</strong></span>`;
        })
}
function kickRaceMember(userId, raceId)
{
    performPostFromButton("KickMember","RaceApi",JSON.stringify({
        userId: userId,
        raceId: raceId
    }),`kickButton${userId}`);
}
function leaveRace(id)
{
    performPostFromButton("Leave","RaceApi",JSON.stringify(id),`leaveButton`);
}
function joinRace(id)
{
    performPostFromButton("Join","RaceApi",JSON.stringify(id),`joinButton`);
}
function kickClubMember(userId, clubId)
{
    performPostFromButton("KickMember","ClubApi",JSON.stringify({
        userId: userId,
        clubId: clubId
    }),`kickButton${userId}`);
}
function showError(message) {
    document.getElementById("errorMessage").innerText = message;
    let toast = new bootstrap.Toast(document.getElementById("errorToast"));
    toast.show();
}
function leaveCLub(id)
{
    performPostFromButton("Leave","ClubApi",JSON.stringify(id),`leaveButton`);
}
function joinClub(id)
{
    performPostFromButton("Join","ClubApi",JSON.stringify(id),`joinButton`);
}