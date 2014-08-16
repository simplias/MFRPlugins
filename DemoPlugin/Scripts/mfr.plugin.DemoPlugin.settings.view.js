mfr.plugin.DemoPlugin = mfr.plugin.DemoPlugin || {};
mfr.plugin.DemoPlugin.settings = mfr.plugin.DemoPlugin.settings || {};

mfr.plugin.DemoPlugin.settings.view = mfr.view.Base.extend({
    template: "DemoPlugin-settings",
    commandBarTemplate: "DemoPlugin-settings-commandBar",

    _initialize: function () {
        _.bindAll(this);
    },

    refresh: function () {

    },

    afterRender: function () {

    },
    events: {
        "click #update-command": "updateCommand",
    },

    updateCommand: function () {
        this.model.save();
        return false;
    },
});
