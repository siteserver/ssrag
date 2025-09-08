from .__base import DatasetRequest, DatasetResult
from repositories import dataset_repository, site_repository


async def flow_dataset(request: DatasetRequest) -> DatasetResult:
    sites = site_repository.get_summaries()
    selected_site_ids = dataset_repository.get_selected_site_ids(
        request.siteId, request.nodeId
    )
    datasetSites = [site for site in sites if site.id in selected_site_ids]

    return DatasetResult(datasetSites=datasetSites)
