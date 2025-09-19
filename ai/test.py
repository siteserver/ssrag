from repositories import model_provider_repository
from enums import ModelType
from extensions import ToImageFactory

providerId = "siliconflow"
modelId = "Qwen/Qwen-Image"
prompt = "一则简约且富有创意的广告，设置在纯白背景上。一个真实的德芙巧克力块与手绘黑色墨水涂鸦相结合，线条松散而俏皮。涂鸦描绘了：巧克力一角“融化”成丝绸瀑布，一群穿礼服的小可可豆乘着丝带滑梯优雅滑落，其中一只用巧克力酱在空中写“纵享丝滑”书法，另一只正把爱心形状的奶油云朵送给路过的月亮。在顶部或中部加入粗体紫色 “融化，是温柔的叛逆” 文字。在底部清晰放置德芙紫色飘带“DOVE”标志。视觉效果应简洁、有趣、高对比度且构思巧妙"

model_credentials = model_provider_repository.get_model_credentials(
    ModelType.TO_IMAGE, providerId, modelId
)
if model_credentials is None:
    raise Exception("模型不存在！")

llm = ToImageFactory.create(model_credentials)
images = llm.generate(prompt, "1024x1024", 1)
print(images)