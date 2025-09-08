from enum import Enum


class OutputFormat(str, Enum):
    """
    Enumeration of output formats.
    """

    MARKDOWN = "Markdown"
    JSON = "JSON"

    @classmethod
    def get_display_names(cls) -> dict[str, str]:
        return {
            cls.MARKDOWN: "Markdown",
            cls.JSON: "JSON",
        }

    @property
    def display_name(self) -> str:
        """
        Returns the display name of the output format.
        """
        return self.get_display_names().get(self.value, self.value)
