var $url = "/apps/create";

var data = utils.init({
  pageType: "process",

  form: {
    uuid: utils.getQueryString("uuid"),
    description: utils.getQueryString("description"),
    iconUrl: utils.getQueryString("iconUrl"),
    createType: utils.getQueryString("createType"),
    root: utils.getQueryBoolean("root"),
    siteDir: utils.getQueryString("siteDir"),
    siteName: utils.getQueryString("siteName"),
    siteTemplate: utils.getQueryString("siteTemplate"),
    siteType: utils.getQueryString("siteType"),
  },

  total: 1,
  current: 0,
  message: "",
  success: false,

  errorMessage: "",
});

var methods = {
  apiSubmit: function () {
    var $this = this;

    var interval = setTimeout(function () {
      $this.apiProcess();
    }, 3000);

    $api
      .post($url, this.form)
      .then(function (response) {
        var res = response.data;

        if (res.value) {
          $this.apiClearCache(res.value);
        }

        // $this.total = 1;
        // $this.current = 1;
        // $this.message = "网站导入成功！";
        // $this.success = true;
        // setTimeout(function () {
        //   parent.location.href = utils.getIndexUrl({ siteId: res.value });
        // }, 1000);
      })
      .catch(function (error) {
        clearTimeout(interval);
        $this.pageType = "error";
        $this.errorMessage = utils.getErrorMessage(error);
      });
  },

  apiClearCache: function (siteId) {
    var $this = this;

    $aiApi.post("/admin/apps/actions/clearCache").then(function (response) {
      var res = response.data;
      if (res.value) {
        $aiApi.tasksIndex(siteId, 0, 0).then(function (response) {
          $this.total = 1;
          $this.current = 1;
          $this.message = "网站导入成功！";
          $this.success = true;
          setTimeout(function () {
            parent.location.href = utils.getIndexUrl({ siteId: siteId });
          }, 1000);
        });
      }
    });
  },

  apiProcess: function () {
    var $this = this;
    $this.pageType = "process";

    $api
      .post($url + "/actions/process", {
        uuid: this.form.uuid,
      })
      .then(function (response) {
        var res = response.data;
        if ($this.success) return;

        $this.total = res.total || 1;
        $this.current = res.current;
        $this.message = res.message;

        if ($this.current > $this.total) {
          $this.current = $this.total;
        }

        if ($this.total > $this.current) {
          setTimeout(function () {
            $this.apiProcess();
          }, 3000);
        }
      })
      .catch(function (error) {
        utils.error(error);
      });
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    utils.loading(this, false);
    this.apiSubmit();
  },
});
