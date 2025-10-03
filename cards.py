class Card:
    def __init__(self, x, y, width, height, card_type, name, cost, cooldown):
        {{ ... }}
        
    def draw(self, surface):
        {{ ... }}
        # Draw banana cost with icon
        banana_icon = font.render("🍌", True, (255, 255, 0))
        cost_text = font.render(str(self.cost), True, (255, 255, 0))
        surface.blit(banana_icon, (self.rect.x + 5, self.rect.y + 5))
        surface.blit(cost_text, (self.rect.x + 25, self.rect.y + 5))
