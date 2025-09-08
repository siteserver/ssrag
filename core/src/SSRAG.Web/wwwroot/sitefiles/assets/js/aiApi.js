var $aiApi = axios.create({
  baseURL: "/api/ai",
  headers: {
    Authorization: "Bearer " + $token,
  },
});

$aiApi.tasksIndex = function (siteId, channelId, contentId) {
  return $aiApi.post("/admin/dataset/tasks/actions/index", {
    siteId: siteId,
    channelId: channelId,
    contentId: contentId,
  });
};

$aiApi.tasksRemove = function (siteId, channelId, contentId) {
  return $aiApi.post("/admin/dataset/tasks/actions/remove", {
    siteId: siteId,
    channelId: channelId,
    contentId: contentId,
  });
};
