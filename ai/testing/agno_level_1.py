# from agno.agent import Agent
# from agno.models.deepseek import DeepSeek
# from agno.tools.reasoning import ReasoningTools
# from agno.tools.yfinance import YFinanceTools

# reasoning_agent = Agent(
#     model=DeepSeek(id="deepseek-r1-distill-qwen-32b"),
#     tools=[
#         ReasoningTools(add_instructions=True),
#         YFinanceTools(
#             stock_price=True,
#             analyst_recommendations=True,
#             company_info=True,
#             company_news=True,
#         ),
#     ],
#     instructions="Use tables to display data.",
#     markdown=True,
# )
