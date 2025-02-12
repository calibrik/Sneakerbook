
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
            }
            else {
                showSuccess(data.message);
            }
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
            document.getElementById("editButton").remove();
            document.getElementById("removeOnDeleteDiv").remove();
            document.getElementById("card").innerHTML=`<span class="text-center text-danger"><strong>Club is removed.</strong></span>`;
        });
}
function kickRaceMember(userId, raceId)
{
    performPostFromButton("KickMember","RaceApi",JSON.stringify({
        userId: userId,
        raceId: raceId
    }),`kickButton${userId}`)
        .then(({response,data})=>{
            if (!response.ok) {
                return;
            }
            document.getElementById(`member${userId}`).remove();
            document.getElementById('membersCount').innerHTML=`${data.memberCount}`;
        });
}
function leaveRace(raceId,userId)
{
    performPostFromButton("Leave","RaceApi",JSON.stringify(raceId),`leaveButton`)
        .then(({response,data})=>{
            if (!response.ok) {
                return;
            }
            document.getElementById(`member${userId}`).remove();
            document.getElementById('membersCount').innerHTML=`${data.memberCount}`;
            document.getElementById(`leaveButton`).remove();
            document.getElementById(`infoDiv`).innerHTML+=`<a id="joinButton" onclick="joinRace('${raceId}')" class="btn btn-lg w-25 btn-outline-primary mt-auto">Join</a>`
            let joinedRaceLabels=Array.from(document.getElementsByClassName("joined-race"));
            joinedRaceLabels.forEach((label)=>{
                label.remove();
            })
        });
}
function joinRace(raceId)
{
    performPostFromButton("Join","RaceApi",JSON.stringify(raceId),`joinButton`)
        .then(({response,data})=>{
            if (!response.ok) {
                return;
            }
            document.getElementById(`joinButton`).remove();
            document.getElementById('membersCount').innerHTML=`${data.memberCount}`;
            document.getElementById(`infoDiv`).innerHTML+=`<a id="leaveButton" onclick="leaveRace('${raceId}','${data.model.id}')" class="btn btn-lg w-25 btn-outline-danger mt-auto">Leave</a>`
            document.getElementById(`memberList`).innerHTML+=`
<li class="list-group-item" id="member${data.model.id}">
    <div class="row">
        <div class="col-md-6 d-flex align-content-center">
            <a href="${data.model.linkToDashboard}" class="link-primary">
                ${data.model.userName}
            </a>
        </div>
        <div class="col-md-6 d-flex align-content-center justify-content-end">
            
        </div>
    </div>
</li>`;
        });
        
}
function kickClubMember(userId, clubId)
{
    performPostFromButton("KickMember","ClubApi",JSON.stringify({
        userId: userId,
        clubId: clubId
    }),`kickButton${userId}`)
        .then(({response,data})=>{
            if (!response.ok) {
                return;
            }
            document.getElementById('membersCount').innerHTML=`${data.memberCount} ${data.memberCount==1?"follower":"followers"}`;
            document.getElementById(`member${userId}`).remove();
        });
}
function showError(message) {
    document.getElementById("errorMessage").innerText = message;
    let toast = new bootstrap.Toast(document.getElementById("errorToast"));
    toast.show();
}
function showSuccess(message) {
    document.getElementById("successMessage").innerText = message;
    let toast = new bootstrap.Toast(document.getElementById("successToast"));
    toast.show();
}
function leaveClub(clubId, userId)
{
    performPostFromButton("Leave","ClubApi",JSON.stringify(clubId),`leaveButton`)
        .then(({response,data})=>{
            if (!response.ok) {
                return;
            }
            document.getElementById(`member${userId}`).remove();
            document.getElementById('membersCount').innerHTML=`${data.memberCount} ${data.memberCount==1?"follower":"followers"}`
            document.getElementById(`leaveButton`).remove();
            document.getElementById(`infoDiv`).innerHTML+=`<a id="joinButton" onclick="joinClub('${clubId}')" class="btn btn-lg w-25 btn-outline-primary mt-auto">Join</a>`
            let joinedRaceLabels=Array.from(document.getElementsByClassName("joined-race"));
            joinedRaceLabels.forEach((label)=>{
                label.remove();
            })
        });
}
function joinClub(clubId)
{
    performPostFromButton("Join","ClubApi",JSON.stringify(clubId),`joinButton`)
        .then(({response,data})=>{
            if (!response.ok) {
                return;
            }
            document.getElementById(`joinButton`).remove();
            document.getElementById('membersCount').innerHTML=`${data.memberCount} ${data.memberCount==1?"follower":"followers"}`
            document.getElementById(`infoDiv`).innerHTML+=`<a id="leaveButton" onclick="leaveClub('${clubId}','${data.model.id}')" class="btn btn-lg w-25 btn-outline-danger mt-auto">Leave</a>`
            document.getElementById(`memberList`).innerHTML+=`
<li class="list-group-item" id="member${data.model.id}">
    <div class="row">
        <div class="col-md-6 d-flex align-content-center">
            <a href="${data.model.linkToDashboard}" class="link-primary">
                ${data.model.userName}
            </a>
        </div>
        <div class="col-md-6 d-flex align-content-center justify-content-end">
            
        </div>
    </div>
</li>`;
    });
}
function deleteRace(raceId)
{
    performPostFromButton("Delete","RaceApi",JSON.stringify(raceId),`deleteButton`)
        .then(({response,data})=>{
            if (!response.ok) {
                return;
            }
            document.getElementById("editButton").remove();
            document.getElementById("removeOnDeleteDiv").remove()
            document.getElementById("card").innerHTML=`<span class="text-center text-danger"><strong>Race is removed.</strong></span>`;
        })
}
async function goBack()
{
    console.log("go back history")
    await history.back();
    // location.reload();
    setTimeout(() => {
        console.log("reload")
        location.reload();
    }, 100);
}