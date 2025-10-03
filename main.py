import pygame
import sys
import os
from enum import Enum
from cards import Card, CardType
from monkeys import Team, DartMonkey, TackShooter

# Initialize Pygame
pygame.init()

# Constants
SCREEN_WIDTH = 1280
SCREEN_HEIGHT = 720
FPS = 60

# Colors
WHITE = (255, 255, 255)
BLACK = (0, 0, 0)
RED = (255, 0, 0)
BLUE = (0, 0, 255)
GREEN = (0, 255, 0)
GRAY = (100, 100, 100)

class GameState(Enum):    
    MENU = 1
    PLAYING = 2
    GAME_OVER = 3

class Game:
    def __init__(self):
        self.screen = pygame.display.set_mode((SCREEN_WIDTH, SCREEN_HEIGHT))
        pygame.display.set_caption("Monkey Clash")
        self.clock = pygame.time.Clock()
        self.running = True
        self.state = GameState.MENU
        
        # Game assets will be loaded here
        self.load_assets()
        
        # Game objects
        self.all_units = []
        self.projectiles = []
        self.cards = []
        self.selected_card = None
        self.money = 1000
        self.font = pygame.font.Font(None, 36)
        
        # Initialize cards
        self.init_cards()
        
    def init_cards(self):
        card_width = 80
        card_height = 100
        padding = 10
        start_x = (SCREEN_WIDTH - (4 * (card_width + padding))) // 2
        
        # Create cards for different monkeys and spells
        self.cards = [
            Card(start_x, SCREEN_HEIGHT - card_height - 10, 
                 card_width, card_height, 
                 CardType.MONKEY, "Dart", 100, 5),
            Card(start_x + (card_width + padding), SCREEN_HEIGHT - card_height - 10,
                 card_width, card_height,
                 CardType.MONKEY, "Tack", 150, 7),
            Card(start_x + 2 * (card_width + padding), SCREEN_HEIGHT - card_height - 10,
                 card_width, card_height,
                 CardType.SPELL, "Spike", 200, 10),
            Card(start_x + 3 * (card_width + padding), SCREEN_HEIGHT - card_height - 10,
                 card_width, card_height,
                 CardType.SPELL, "Bomb", 300, 15)
        ]
        
    def load_assets(self):
        # This will be used to load images, sounds, etc.
        self.assets = {}
        # Placeholder for assets loading
        
    def new_game(self):
        # Reset game state
        self.all_units = []
        self.projectiles = []
        self.money = 1000
        self.state = GameState.PLAYING
        
    def handle_events(self):
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                self.running = False
            
            if self.state == GameState.MENU:
                if event.type == pygame.KEYDOWN:
                    if event.key == pygame.K_RETURN:
                        self.new_game()
            
            elif self.state == GameState.PLAYING:
                if event.type == pygame.MOUSEBUTTONDOWN:
                    if event.button == 1:  # Left click
                        mouse_pos = pygame.mouse.get_pos()
                        
                        # Check card selection
                        if not self.selected_card:
                            for card in self.cards:
                                if card.rect.collidepoint(mouse_pos) and card.active and self.money >= card.cost:
                                    self.selected_card = card
                                    break
                        else:
                            # Place unit if a card is selected
                            if 0 <= mouse_pos[0] <= SCREEN_WIDTH // 2:  # Only allow placement on left half
                                if self.selected_card.name == "Dart":
                                    self.all_units.append(DartMonkey(mouse_pos[0], mouse_pos[1], Team.PLAYER))
                                    self.money -= self.selected_card.cost
                                    self.selected_card.use(mouse_pos[0], mouse_pos[1])
                                elif self.selected_card.name == "Tack":
                                    self.all_units.append(TackShooter(mouse_pos[0], mouse_pos[1], Team.PLAYER))
                                    self.money -= self.selected_card.cost
                                    self.selected_card.use(mouse_pos[0], mouse_pos[1])
                                # Add other unit types here
                                
                                self.selected_card = None
                            
                            # Deselect card if clicking outside play area
                            elif mouse_pos[1] < SCREEN_HEIGHT - 120:  # Above card area
                                self.selected_card = None
                
    def update(self):
        if self.state == GameState.PLAYING:
            # Update cards
            for card in self.cards:
                card.update()
            
            # Update units and collect projectiles
            for unit in self.all_units[:]:
                unit.update([u for u in self.all_units if u != unit])
                
                # Collect projectiles
                self.projectiles.extend(unit.projectiles)
                unit.projectiles = []
                
                # Check for unit death
                if unit.health <= 0:
                    self.all_units.remove(unit)
            
            # Update projectiles and check for hits
            for proj in self.projectiles[:]:
                proj.update()
                
                # Check for hits
                for unit in self.all_units:
                    if (unit.team != proj.team and 
                        math.hypot(unit.x - proj.x, unit.y - proj.y) < unit.radius + proj.radius):
                        if unit.take_damage(proj.damage):
                            if unit.team == Team.ENEMY:
                                self.money += 50  # Reward for killing enemy
                        proj.active = False
                        break
                
                if not proj.active:
                    self.projectiles.remove(proj)
            
            # Spawn enemy waves (simple implementation)
            if pygame.time.get_ticks() % 3000 < 30:  # Every ~3 seconds
                self.spawn_enemy_wave()
            
    def draw(self):
        self.screen.fill(BLACK)
        
        if self.state == GameState.MENU:
            self.draw_text("MONKEY CLASH", 64, WHITE, SCREEN_WIDTH // 2, SCREEN_HEIGHT // 4)
            self.draw_text("Press ENTER to start", 32, WHITE, SCREEN_WIDTH // 2, SCREEN_HEIGHT // 2)
            
        elif self.state == GameState.PLAYING:
            # Draw play area
            pygame.draw.rect(self.screen, (50, 50, 70), (0, 0, SCREEN_WIDTH // 2, SCREEN_HEIGHT - 120))
            pygame.draw.rect(self.screen, (70, 50, 50), (SCREEN_WIDTH // 2, 0, SCREEN_WIDTH // 2, SCREEN_HEIGHT - 120))
            
            # Draw units
            for unit in self.all_units:
                unit.draw(self.screen)
            
            # Draw projectiles
            for proj in self.projectiles:
                proj.draw(self.screen)
            
            # Draw UI
            self.draw_ui()
            
            # Draw selected card highlight
            if self.selected_card:
                mouse_pos = pygame.mouse.get_pos()
                pygame.draw.circle(self.screen, (255, 255, 0), mouse_pos, 20, 2)
        
        pygame.display.flip()
    
    def draw_ui(self):
        # Draw card area background
        pygame.draw.rect(self.screen, (40, 40, 50), (0, SCREEN_HEIGHT - 120, SCREEN_WIDTH, 120))
        
        # Draw cards
        for card in self.cards:
            card.draw(self.screen)
            
            # Highlight if selected
            if card == self.selected_card:
                pygame.draw.rect(self.screen, (255, 255, 0), card.rect, 3)
        
        # Draw money
        money_text = self.font.render(f"${self.money}", True, (255, 255, 0))
        self.screen.blit(money_text, (SCREEN_WIDTH - 150, SCREEN_HEIGHT - 110))
    
    def spawn_enemy_wave(self):
        # Simple enemy spawning logic
        from random import randint
        
        # Spawn 1-3 enemies
        for _ in range(randint(1, 3)):
            x = randint(SCREEN_WIDTH // 2, SCREEN_WIDTH - 50)
            y = randint(50, SCREEN_HEIGHT - 170)
            
            # Randomly choose enemy type
            if randint(0, 1) == 0:
                self.all_units.append(DartMonkey(x, y, Team.ENEMY))
            else:
                self.all_units.append(TackShooter(x, y, Team.ENEMY))
        
    def draw_text(self, text, size, color, x, y):
        font = pygame.font.Font(None, size)
        text_surface = font.render(text, True, color)
        text_rect = text_surface.get_rect()
        text_rect.midtop = (x, y)
        self.screen.blit(text_surface, text_rect)
        
    def run(self):
        while self.running:
            self.clock.tick(FPS)
            self.handle_events()
            self.update()
            self.draw()
            
        pygame.quit()
        sys.exit()

if __name__ == "__main__":
    game = Game()
    game.run()
