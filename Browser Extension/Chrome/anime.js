const version = '0.3.1';

/*
 * Information variables
 */
var anime;
var currentEpisode;

/*
 * Get anime name
 */
anime = document.title.split('Episode')[0].trim();

/*
 * Get current episode
 */
var list = document.getElementById('selectEpisode').getElementsByTagName('option');

for (var i = 0; i < list.length; i++) {
    if (list.item(i).hasAttribute('selected'))
        currentEpisode = parseInt(list.item(i).innerText.replace('Episode ', ''));
}

/*
 * Initialize WebSocket
 */
var webSocket = new WebSocket('ws://localhost:8080/KADRP');

/*
 * Prepare the messages now
 */
var animeMessage = '{"action":0,"anime":"' + anime + '","current_episode":' + currentEpisode + '}';
var clearMessage = '{"action":1}';

/*
 * Define the event handlers
 */
function onPlaying(event) {
    webSocket.send(animeMessage);
}

function onPause(event) {
    webSocket.send(clearMessage);
}

function onBeforeUnload(event) {
    webSocket.send(clearMessage);
}

/*
 * Register event handlers
 */
document.getElementById('my_video_1_html5_api').onplaying = onPlaying;
document.getElementById('my_video_1_html5_api').onpause = onPause;
window.onbeforeunload = onBeforeUnload;

/*
 * Get the version of the server
 */
var object;

webSocket.onmessage = function (event) {
    object = JSON.parse(event.data);
};

var serverVersion = object.version;

/*
 * Check if the versions are compatible
 */
if (serverVersion !== version) {
    console.log('Incompatible version! Version ' + serverVersion);
    alert('The dependency service installed is version ' + serverVersion + ', while the addon version is ' + version + '! Please update them both to the latest version, and always make sure that they have the same version number.');
} else {
    console.log('Compatible version! Version ' + serverVersion);
}