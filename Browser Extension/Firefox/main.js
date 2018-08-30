function onCreated(tab) {
    console.log(`Created new tab: ${tab.id}`)
}

function onError(error) {
    console.log(`Error: ${error}`);
}

browser.browserAction.onClicked.addListener(function () {
    var creating = browser.tabs.create({
        url: "https://github.com/AnjoEscuro/KissAnime-Discord-Rich-Presence/releases"
    });

    creating.then(onCreated, onError);
});