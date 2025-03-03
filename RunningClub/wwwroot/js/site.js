function getTimezone()
{
    document.getElementById("timezoneInput").value=Intl.DateTimeFormat().resolvedOptions().timeZone;
}
function convertTimeToLocalForInputs()
{
    let utcDate= document.getElementById("fullDate").value;
    let localDate=new Date(utcDate+"Z");
    let inputs=Array.from(document.getElementsByClassName("timeInput"));
    inputs.forEach((element)=>{
        const hours = String(localDate.getHours()).padStart(2, '0');
        const minutes = String(localDate.getMinutes()).padStart(2, '0');
        element.value= `${hours}:${minutes}:00.000`;
    });
}
function convertDateToLocalForInputs()
{
    let utcDate= document.getElementById("fullDate").value;
    let localDate=new Date(utcDate+"Z");
    let inputs=Array.from(document.getElementsByClassName("dateInput"));
    inputs.forEach((element)=>{
        const year = localDate.getFullYear();
        const month = String(localDate.getMonth() + 1).padStart(2, '0'); // Months are zero-based
        const day = String(localDate.getDate()).padStart(2, '0');
        console.log(localDate);
        element.value= `${year}-${month}-${day}`;
        console.log(element.value);
    });
}
function showLocalTime(utcDateTime)
{
    const localTime = new Date(utcDateTime+'Z');
    const weekdays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
    const months=['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
    const dayOfWeek = weekdays[localTime.getDay()];
    const day = localTime.getDate().toString();
    const month = months[localTime.getMonth()];
    const year = localTime.getFullYear();
    let hours = localTime.getHours();
    const minutes = localTime.getMinutes().toString().padStart(2, '0');
    const ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = !hours&&ampm=="PM" ? 12 : hours;
    hours=hours.toString();
    return `${dayOfWeek} ${day} ${month} ${year}, at ${hours}:${minutes} ${ampm}`;
}
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
                                    <small class="text-muted">Starts on ${showLocalTime(race.startDate)}</small>
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
function buildRacesList(data)
{
    let raceList=document.getElementById("raceList");
    raceList.innerHTML="";
    data.forEach((race)=>{
        raceList.innerHTML += `
<div class="card mb-4 d-flex flex-column" style="height: 15%;">
        <div class="row g-0 h-100">
        <div class="col-md-4 d-flex">
        <div class="d-flex justify-content-center">
        <div id="spinner ${race.id}" class="spinner-border m-5" role="status">
        <span class="visually-hidden">Loading...</span>
</div>
    <img id="image ${race.id}" style="display: none" src="${race.image}" class="img-fluid" alt="Image ${race.id}" onload="onImageLoadById('${race.id}')">
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
            <p class="text-muted"><small>Starts on ${showLocalTime(race.startDate)}</small></p>

            <div class="mt-auto d-flex flex-column">
                <small class="text-muted">
                    Created by <a class="link-info" href="${race.clubLink}">${race.clubTitle}</a>
                </small>
                <a class="btn w-25 btn-sm btn-primary mt-2" href="${race.raceLink}">View</a>
            </div>
        </div>
    </div>
</div>

    <!-- Footer remains for participants count -->
    <div class="card-footer d-flex justify-content-end">
        <small class="text-muted">Participants ${race.memberCount}/${race.maxMemberCount}</small>
    </div>
</div>`;
    });
}
function buildMemberList(data,kickFunction)
{
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
                label=`<a id="kickButton${member.id}" class="btn btn-danger" onclick="${kickFunction.name}('${member.id}','${data.id}')"><strong class="text-white">Kick</strong></a>`;
            }
            memberList.innerHTML += `
<li class="list-group-item" id="member${member.id}">
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
                button = `<a id="deleteButton" onclick="deleteClub('${data.id}')" class="btn btn-lg w-25 btn-outline-danger mt-auto">Delete Club</a>`;
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
            
            buildMemberList(data,kickClubMember);
        });
}
function getRaces()
{
    let raceList=document.getElementById("raceList");
    raceList.innerHTML=spinner();
    performGet("GetRaces","RaceApi")
        .then(({response,data})=>{
            if (!response.ok)
            {
                return;
            }
            raceList.innerHTML=``;
            if (data.length==0) {
                raceList.innerHTML = `<p class="text-muted text-center">No available races at the moment. Join more clubs to see more races!</p>`
                return;
            }
            buildRacesList(data);
        });
}

function getRaceInfo(raceId)
{
    let raceInfo=document.getElementById("raceInfo");
    raceInfo.innerHTML=spinner();
    document.getElementById("memberList").innerHTML=spinner();
    performGet("GetRace","RaceApi",new URLSearchParams({ raceId:raceId }).toString())
    .then(({response,data})=>{
        if (!response.ok)
        {
            return;
        }
        let button="";
        if (data.isCompleted)
            button=`<p class="text-danger"><strong>Race has finished</strong></p>`;
        else if (!data.isJoined)
            button=`<a id="joinButton" onclick="joinRace('${data.id}')" class="btn btn-lg w-25 btn-outline-primary mt-auto">Join</a>`;
        else if (data.isAdmin)
            button=`<a id="deleteButton" onclick="deleteRace('${data.id}')" class="btn btn-lg w-25 btn-outline-danger mt-auto">Delete Race</a>`;
        else
            button=`<a id="leaveButton" onclick="leaveRace('${data.id}','@User.GetUserId()')" class="btn btn-lg w-25 btn-outline-danger mt-auto">Leave</a>`;
        raceInfo.innerHTML=`
<div class="col-md-6">
    <!-- Club Image -->
    <div class="d-flex justify-content-center">
        <div id="spinner" class="spinner-border m-5" role="status">
            <span class="visually-hidden">Loading...</span>
        </div>
    </div>
    <img id="image" style="display: none" src="${data.image}" class="img-fluid" alt="Club Image" onload="onImageLoad()">
</div>
<div class="col-md-6" id="infoDiv">
    <!-- Club Information -->
    <h1 class="display-4">${data.title}</h1>
    <h4 class="text-muted">Category: <span class="badge bg-primary">${data.category}</span></h4>
    <p class="card-text"><strong>Address: </strong> ${data.address.country}, ${data.address.city}, ${data.address.street} </p>
    <p class="card-text"><strong>Description: </strong>${data.description}</p>
    <p class="card-text"><strong>Number of Participants: </strong><span id="membersCount">${data.members.length}</span>/${data.maxMembersNumber}</p>
    <p class="card-text"><strong>Created by: </strong><a href="${data.adminLink}" class="link-primary">${data.adminUsername}</a></p>
    <p class="card-text"><strong>Hosting Club: </strong><a href="${data.clubLink}" class="link-primary">${data.clubTitle}</a></p>
    <p class="text-muted"><strong>${data.isCompleted?"Finished":"Starts"} on ${showLocalTime(data.startDate)}</strong></p>
    ${button}
</div>`;
        buildMemberList(data,kickRaceMember);
    })
}

function getUserInfo(userId)
{
    let userInfo=document.getElementById("userInfo");
    userInfo.innerHTML=spinner();
    performGet("GetUserInfo","DashboardApi",new URLSearchParams({ userId: userId }).toString())
        .then(({response,data})=>{
            if (!response.ok)
            {
                userInfo.innerHTML=`<span class="text-danger text-center"><strong>${data.message}</strong></span>`;
                return;
            }
            userInfo.innerHTML=`
<h2 class="card-title text-center mb-4">${data.username}</h2>
    <p><strong>Name:</strong> ${data.fName} ${data.lName}</p>
    <p><strong>Email:</strong> ${data.email}</p>
    <p><strong>Mileage:</strong> ${data.mileage} km</p>
    ${data.isAdmin?`<p><strong class="text-danger">Admin</strong></p>`:""}
    ${data.isSelf?`
        <a href="${data.changePasswordLink}" class="mt-3 btn btn-outline-secondary w-25">Change Password</a>
        <div class="mt-auto d-flex justify-content-center">
            <a href="${data.editProfileLink}" class="btn btn-primary w-50">Edit Profile</a>
        </div>`
                :""}`
            
        });
}

function getUserUpcomingRaces(userId)
{
    let raceList=document.getElementById("raceList");
    raceList.innerHTML=spinner();
    performGet("GetUsersUpcomingRaces","DashboardApi",new URLSearchParams({ userId: userId }).toString())
        .then(({response,data})=>{
            if (!response.ok)
            {
                raceList.innerHTML="";
                return;
            }
            if (data.length==0)
            {
                raceList.innerHTML=`<p class="text-muted text-center">No upcoming races at the moment.</p>`;
                return;
            }
            buildRacesList(data);
        })
}
function getUserAdminRaces(userId)
{
    let raceList=document.getElementById("raceList");
    raceList.innerHTML=spinner();
    performGet("GetUsersAdminRaces","DashboardApi",new URLSearchParams({ userId: userId }).toString())
        .then(({response,data})=>{
            if (!response.ok)
            {
                raceList.innerHTML="";
                return;
            }
            if (data.length==0)
            {
                raceList.innerHTML=`<p class="text-muted text-center">You didn't create any races so far.</p>`;
                return;
            }
            buildRacesList(data);
        })
}
function getUserCompletedRaces(userId)
{
    let raceList=document.getElementById("raceList");
    raceList.innerHTML=spinner();
    performGet("GetUsersCompletedRaces","DashboardApi",new URLSearchParams({ userId: userId }).toString())
        .then(({response,data})=>{
            if (!response.ok)
            {
                raceList.innerHTML="";
                return;
            }
            if (data.length==0)
            {
                raceList.innerHTML=`<p class="text-muted text-center">You didn't complete any races so far.</p>`;
                return;
            }
            buildRacesList(data);
        })
}
function buildClubsList(data)
{
    let clubList=document.getElementById("clubList");
    clubList.innerHTML="";
    data.forEach((club)=>{
        clubList.innerHTML+=`
<div class="card mb-3">
    <div class="row g-0">
        <div class="col-md-4">
            <div class="d-flex justify-content-center">
                <div id="spinner ${club.id}" class="spinner-border m-5" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
            </div>
            <img id="image ${club.id}" style="display: none" src=${club.image} class="img-fluid" alt="Club Image" onload="onImageLoadById('${club.id}')">
        </div>
        <div class="col-md-8">
            <div class="card-body">
                <h5 class="card-title">${club.title}</h5>
                <p class="text-muted">${club.category}</p>
                <p class="card-text">${club.description}</p>
                <div class="mt-auto d-flex flex-column">
                    <small class="text-muted">
                        Created by <a class="link-info" href="${club.adminLink}">${club.adminUsername}</a>
                    </small>
                    <a href="${club.clubLink}" class="btn w-25 btn-sm btn-primary mt-2">View</a>
                </div>
            </div>
        </div>
    </div>
</div>`
    });
}

function getUserClubs(userId)
{
    let clubList=document.getElementById("clubList");
    clubList.innerHTML=spinner();
    performGet("GetUserClubs","DashboardApi",new URLSearchParams({ userId: userId }).toString())
        .then(({response,data})=>{
            if (!response.ok)
            {
                clubList.innerHTML="";
                return;
            }
            if (data.length==0)
            {
                clubList.innerHTML=`<p class="text-muted text-center">You aren't a participant in any club</p>`;
                return;
            }
            buildClubsList(data);
        })
}

function getUserAdminClubs(userId)
{
    let clubList=document.getElementById("clubList");
    clubList.innerHTML=spinner();
    performGet("GetUserAdminClubs","DashboardApi",new URLSearchParams({ userId: userId }).toString())
        .then(({response,data})=>{
            if (!response.ok)
            {
                clubList.innerHTML="";
                return;
            }
            if (data.length==0)
            {
                clubList.innerHTML=`<p class="text-muted text-center">You aren't an admin in any club</p>`;
                return;
            }
            buildClubsList(data);
        })
}
//TODO load club and races on scroll