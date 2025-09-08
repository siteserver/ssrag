﻿var $url = "/common/userLayerView";

var data = utils.init({
  uuid: utils.getQueryString("uuid"),
  user: null,
  groups: null,
  departmentFullName: null,
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .get($url, {
        params: {
          uuid: this.uuid,
        },
      })
      .then(function (response) {
        var res = response.data;

        $this.user = res.user;
        $this.groups = res.groups;
        $this.departmentFullName = res.departmentFullName;
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(null, this.btnCancelClick);
    this.apiGet();
  },
});
