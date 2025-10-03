"""
Card system with banana cost display
"""
import pygame

class Card:
    def __init__(self, x, y, width, height, card_type, name, cost, cooldown):
        self.rect = pygame.Rect(x, y, width, height)
        self.cost = cost
        
    def draw(self, surface):
        """Draw card with banana cost"""
        # Banana icon
        font = pygame.font.Font(None, 24)
        banana_icon = font.render("🍌", True, (255, 215, 0))
        surface.blit(banana_icon, (self.rect.x + 5, self.rect.y + 5))
        
        # Cost number
        cost_text = font.render(str(self.cost), True, (255, 255, 255))
        surface.blit(cost_text, (self.rect.x + 25, self.rect.y + 5))
