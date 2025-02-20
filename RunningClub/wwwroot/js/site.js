
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
    button.innerHTML=spinner();
    try {
        let response=await fetch(`/api/${controller}/${action}`,
            {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: body,
            });
            let data =await response.json();
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
    await window.history.back();
}
async function performGet(action, controller, queryParams="")
{
    try {
        let response=await fetch(`/api/${controller}/${action}?${queryParams}`,
            {
                method: 'GET',
                headers: {'Content-Type': 'application/json'},
            });
        let data =await response.json();
        if (!response.ok) {
            showError(data.message);
        }
        return {response,data};
    }
    catch(error) {
        console.log(error);
    }
}
function getClubCompletedRaces(clubId)
{
    document.getElementById("raceList").innerHTML=spinner();
    performGet("GetCompletedRacesForClub","ClubApi",new URLSearchParams({ clubId:clubId }).toString())
        .then(({response,data})=>{
            if (!response.ok) {
                document.getElementById("raceList").innerHTML=`<p class="text-center text-danger"><strong>Error.</strong></p>`
                return;
            }
            document.getElementById("raceListTitle").innerText="Completed Races";
            document.getElementById("raceCount").innerText=`${data.length} ${data.length==1 ? "race" : "races"}`;
            if (data.length==0)
            {
                document.getElementById("raceList").innerHTML=`<p class="text-muted text-center">No races were completed for this club.</p>`;
                return;
            }
            buildClubRacesList(data);
        })
}
function spinner()
{
    return `
<div class="d-flex justify-content-center">
  <div class="spinner-border" role="status">
    <span class="visually-hidden">Loading...</span>
  </div>
</div>`
}
function getClubUpcomingRaces(clubId)
{
    document.getElementById("raceList").innerHTML=spinner();
    performGet("GetUpcomingRacesForClub","ClubApi",new URLSearchParams({ clubId:clubId }).toString())
        .then(({response,data})=>{
            if (!response.ok) {
                document.getElementById("raceList").innerHTML=`<p class="text-center text-danger"><strong>Error.</strong></p>`
                return;
            }
            document.getElementById("raceListTitle").innerText="Upcoming Races";
            document.getElementById("raceCount").innerText=`${data.length} ${data.length==1 ? "race" : "races"}`;
            if (data.length==0)
            {
                document.getElementById("raceList").innerHTML=`<p class="text-muted text-center">No races were created for this club.</p>`;
                return;
            }
            buildClubRacesList(data);
        })
}
function buildClubRacesList(array)
{
    document.getElementById("raceList").innerHTML=``;
    array.forEach(race => {
        document.getElementById("raceList").innerHTML += `
                <div class="card mb-4 d-flex flex-column" style="height: 15%;">
                    <div class="row g-0 h-100">
                        <div class="col-md-4 d-flex">
                            <div class="d-flex justify-content-center">
                                <div id="spinner ${race.id}" class="spinner-border m-5" role="status">
                                    <span class="visually-hidden">Loading...</span>
                                </div>
                                <img id="image ${race.id}" style="display: none" src=${race.image} class="img-fluid" alt="Image ${race.id}" onload="onImageLoadById(${race.id})">
                            </div>
                        </div>
                        <div class="col-md-8 d-flex">
                            <div class="card-body d-flex flex-column">
                                <div class="d-flex justify-content-between align-items-center">
                                    <h5 class="card-title">${race.title}</h5>
                                    <span class="text-muted">Distance: ${race.length} km</span>
                                </div>
                                <h6 class="text-muted">${race.category}</h6>
                                <p class="card-text">${race.description}</p>
                                <div class="mt-auto d-flex flex-column">
                                    ${race.isJoined?`<p class="text-warning joined-race">Joined</p>`:""}
                                    <small class="text-muted">Starts on ${race.startDate}</small>
                                    <a class="btn w-25 btn-sm btn-primary mt-2" href="${race.link}">View</a>
                                </div>
                            </div>
                            <div class="card-footer d-flex justify-content-center align-items-center">
                                <small class="text-muted">Participants ${race.memberCount}/${race.maxMemberCount}</small>
                            </div>
                        </div>
                    </div>
                </div>`
    });
}
function buildClubsListForClubIndex(array)
{
    let divToFill=document.getElementById("clubList");
    let content=[];
    content.push(`<div class="row row-cols-1 row-cols-md-3 g-4">`);

    array.forEach(club => {
        content.push(`
<div class="col">
    <div class="card h-100">
        <div class="d-flex justify-content-center">
            <div id="spinner ${club.id}" class="spinner-border m-5" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>
        <img id="image ${club.id}" style="display: none" src="${club.image}" class="card-img-top card-img-custom" alt="Image ${club.id}" onload="onImageLoadById(${club.id})">

        <div class="card-body d-flex flex-column">
            <h5 class="card-title">${club.title}</h5>
            <h6 class="text-muted">${club.category}</h6>
            <p class="text-warning">${club.isJoined?"Joined":""}</p>
            <div class="mt-auto d-flex justify-content-between align-items-center">
                <a class="btn btn-sm btn-outline-primary w-25" href="${club.link}">View</a>
                <p class="text-muted mb-0">${club.memberCount} ${club.memberCount == 1 ? "follower" : "followers"}</p>
            </div>
        </div>

        <div class="card-footer">
            <small class="text-muted">Created by <a class="link-info" href="${club.adminLink}">${club.adminUsername}</a></small>
        </div>
    </div>
</div>`);
    });
    divToFill.innerHTML =content.join("");
}
function onChangeInClubRacesSelector(clubId)
{
    let value=document.getElementById("raceSelector").value;
    if (value=="upcoming")
        getClubUpcomingRaces(clubId);
    else
        getClubCompletedRaces(clubId);
}

function getClubs()
{
    document.getElementById("clubList").innerHTML=spinner();
    performGet("GetClubs","ClubApi")
        .then(({response,data})=>{
            if (!response.ok)
            {
                return;
            }
            buildClubsListForClubIndex(data.clubs);
        })
}
function getClubInfo(clubId)
{
    document.getElementById("clubInfo").innerHTML=spinner();
    document.getElementById("memberList").innerHTML=spinner();
    performGet("GetClub","ClubApi",new URLSearchParams({ clubId:clubId }).toString())
        .then(({response,data})=>{
            if (!response.ok)
            {
                return;
            }
            let button="";
            if (data.isAdmin)
            {
                button=`<a id="deleteButton" onclick="deleteClub('${data.id}')" class="btn btn-lg w-25 btn-outline-danger mt-auto">Delete Club</a>`;
            }
            else if (data.isJoined)
            {
                button=`<a id="leaveButton" onclick="leaveClub('${data.id}','${data.adminId}')" class="btn btn-lg w-25 btn-outline-danger mt-auto">Leave</a>`
            }
            else
            {
                button=`<a id="joinButton" onclick="joinClub('${data.id}')" class="btn btn-lg w-25 btn-outline-primary mt-auto">Join</a>`
            }
            document.getElementById("clubInfo").innerHTML=`
<div class="row">
    <div class="col-md-6">
        <!-- Club Image -->
        <div class="d-flex justify-content-center">
            <div id="spinner" class="spinner-border m-5" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>
        <img id="image" style="display: none" src=${data.image} class="img-fluid rounded-1" alt="Club Image" onload="onImageLoad()">
    </div>
    <div class="col-md-6" id="infoDiv">
        <!-- Club Information -->
        <h1 class="display-4">${data.title}</h1>
        <h4 class="text-muted">Category: <span class="badge bg-primary">${data.category}</span></h4>
        <p class="card-text"><strong>Address: </strong> ${data.address.country}, ${data.address.city}, ${data.address.street} </p>
        <p class="card-text"><strong>Description: </strong>${data.description}</p>
        <p class="card-text"><strong>Created by: </strong><a href="${data.adminLink}" class="link-primary">${data.adminUsername}</a></p>
        <p id="membersCount" class="text-muted">${data.members.length} ${data.members.length==1?"follower":"followers"}</p>
        ${button}
    </div>
</div>`;
            
            let memberList=document.getElementById("memberList");
            if (data.members.length==0)
                memberList.innerHTML=`<p class="text-muted text-center">No members joined this club yet.</p>`
            else {
                memberList.innerHTML = "";
                data.members.forEach(member=>{
                    let label="";
                    if (member.isAdmin)
                    {
                        label=`<p><strong class="text-danger">Admin</strong></p>`;
                    }
                    else if(data.isAdmin) {
                        label=`<a id="kickButton${member.id}" class="btn btn-danger"
                           onClick="kickClubMember('${member.id}','${data.id}')"><strong class="text-white">Kick</strong></a>`;
                    }
                    memberList.innerHTML += `
<li class="list-group-item" id="member@(user.Id)">
    <div class="row">
        <div class="col-md-6 d-flex align-content-center">
            <a href="${member.link}" class="link-primary">
                ${member.username}
            </a>
        </div>
        <div class="col-md-6 d-flex align-content-center justify-content-end">
            ${label}
        </div>
    </div>
</li>`
                });
            }
        });
}

//TODO load club and races on scroll