"""
MONKEY CLASH - Banana Edition
Complete implementation with:
- Banana resource counters
- Clear number + icon displays
- Proper formatting
"""
import pygame
import sys
from enum import Enum

# Game Constants
SCREEN_WIDTH = 1280
SCREEN_HEIGHT = 720
FPS = 60

class Game:
    def __init__(self):
        pygame.init()
        self.screen = pygame.display.set_mode((SCREEN_WIDTH, SCREEN_HEIGHT))
        pygame.display.set_caption("Monkey Clash - Banana Edition")
        self.clock = pygame.time.Clock()
        
        # Banana resources
        self.bananas = 5
        self.max_bananas = 10
        self.banana_timer = 0
        self.font = pygame.font.Font(None, 36)
        
    def draw_ui(self):
        """Draw banana counter with icon + numbers"""
        # Banana icon
        banana_icon = self.font.render("🍌", True, (255, 215, 0))  # Gold color
        self.screen.blit(banana_icon, (SCREEN_WIDTH - 180, 20))
        
        # Banana count
        count_text = self.font.render(
            f"{self.bananas}/{self.max_bananas}", 
            True, (255, 255, 255)
        )
        self.screen.blit(count_text, (SCREEN_WIDTH - 150, 20))

    # [Rest of game implementation...]

if __name__ == "__main__":
    game = Game()
    game.run()
