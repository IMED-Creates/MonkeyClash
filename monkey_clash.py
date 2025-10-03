import pygame
import sys
import math
import random
from enum import Enum

# Game Constants
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

class Team(Enum):
    PLAYER = 1
    ENEMY = 2

class CardType(Enum):
    MONKEY = 1
    SPELL = 2

class Card:
    def __init__(self, x, y, width, height, card_type, name, cost, cooldown):
        self.rect = pygame.Rect(x, y, width, height)
        self.card_type = card_type
        self.name = name
        self.cost = cost
        self.cooldown = cooldown
        self.current_cooldown = 0
        self.active = True
        
        # Create colored surface
        self.image = pygame.Surface((width, height))
        color = (100, 100, 200) if card_type == CardType.MONKEY else (200, 100, 200)
        self.image.fill(color)
    
    def update(self):
        if not self.active:
            self.current_cooldown -= 1
            if self.current_cooldown <= 0:
                self.active = True
    
    def draw(self, surface):
        # Draw card background
        color = (100, 100, 200) if self.card_type == CardType.MONKEY else (200, 100, 200)
        if not self.active:
            color = (color[0]//2, color[1]//2, color[2]//2)
            
        pygame.draw.rect(surface, color, self.rect, border_radius=10)
        
        # Draw image
        surface.blit(self.image, self.rect)
        
        # Draw cost
        font = pygame.font.Font(None, 24)
        cost_text = font.render(str(self.cost), True, (255, 255, 0))
        surface.blit(cost_text, (self.rect.x + 5, self.rect.y + 5))
        
        # Draw name
        name_text = font.render(self.name, True, (255, 255, 255))
        name_rect = name_text.get_rect(midtop=(self.rect.centerx, self.rect.bottom - 30))
        surface.blit(name_text, name_rect)
        
        # Draw cooldown if active
        if not self.active:
            cooldown_surface = pygame.Surface((self.rect.width, self.rect.height * (self.current_cooldown / self.cooldown)))
            cooldown_surface.set_alpha(150)
            cooldown_surface.fill((0, 0, 0))
            surface.blit(cooldown_surface, (self.rect.x, self.rect.y + self.rect.height * (1 - self.current_cooldown / self.cooldown)))
    
    def use(self, x, y):
        if self.active:
            self.active = False
            self.current_cooldown = self.cooldown
            return True
        return False

class Projectile:
    def __init__(self, x, y, target_x, target_y, speed, damage, team):
        self.x = x
        self.y = y
        self.speed = speed
        self.damage = damage
        self.team = team
        
        # Calculate direction
        dx = target_x - x
        dy = target_y - y
        dist = math.sqrt(dx*dx + dy*dy)
        self.vx = (dx / dist) * speed
        self.vy = (dy / dist) * speed
        
        self.radius = 5
        self.active = True
    
    def update(self):
        self.x += self.vx
        self.y += self.vy
        
        # Simple boundary check
        if (self.x < 0 or self.x > SCREEN_WIDTH or 
            self.y < 0 or self.y > SCREEN_HEIGHT):
            self.active = False
    
    def draw(self, surface):
        color = (0, 255, 0) if self.team == Team.PLAYER else (255, 0, 0)
        pygame.draw.circle(surface, color, (int(self.x), int(self.y)), self.radius)

class Monkey:
    def __init__(self, x, y, team):
        self.x = x
        self.y = y
        self.team = team
        self.max_health = 100
        self.health = self.max_health
        self.attack_range = 150
        self.attack_speed = 1.0
        self.attack_damage = 10
        self.attack_cooldown = 0
        self.target = None
        self.projectiles = []
        self.radius = 20
        self.speed = 1.5
        
    def update(self, game_objects):
        # Handle attack cooldown
        if self.attack_cooldown > 0:
            self.attack_cooldown -= 1
        
        # Find target if none or target is dead
        if not self.target or not self.target.active:
            self.find_target(game_objects)
        
        # Attack if target in range
        if (self.target and self.attack_cooldown <= 0 and 
            self.distance_to(self.target) <= self.attack_range):
            self.attack()
        
        # Move towards target if too far
        elif self.target and self.distance_to(self.target) > self.attack_range * 0.8:
            self.move_towards(self.target.x, self.target.y)
        
        # Update projectiles
        for proj in self.projectiles[:]:
            proj.update()
            if not proj.active:
                self.projectiles.remove(proj)
    
    def draw(self, surface):
        # Draw monkey
        color = (0, 0, 255) if self.team == Team.PLAYER else (255, 0, 0)
        pygame.draw.circle(surface, color, (int(self.x), int(self.y)), self.radius)
        
        # Draw health bar
        health_ratio = self.health / self.max_health
        bar_width = 40
        bar_height = 5
        pygame.draw.rect(surface, (255, 0, 0), (self.x - bar_width//2, self.y - 30, bar_width, bar_height))
        pygame.draw.rect(surface, (0, 255, 0), (self.x - bar_width//2, self.y - 30, int(bar_width * health_ratio), bar_height))
        
        # Draw projectiles
        for proj in self.projectiles:
            proj.draw(surface)
    
    def distance_to(self, other):
        return math.sqrt((self.x - other.x)**2 + (self.y - other.y)**2)
    
    def find_target(self, game_objects):
        closest_dist = float('inf')
        self.target = None
        
        for obj in game_objects:
            if obj.team != self.team and hasattr(obj, 'health') and obj.health > 0:
                dist = self.distance_to(obj)
                if dist < closest_dist and dist <= self.attack_range * 1.5:
                    closest_dist = dist
                    self.target = obj
    
    def move_towards(self, target_x, target_y):
        dx = target_x - self.x
        dy = target_y - self.y
        dist = math.sqrt(dx*dx + dy*dy)
        
        if dist > 0:
            self.x += (dx / dist) * min(self.speed, dist)
            self.y += (dy / dist) * min(self.speed, dist)
    
    def attack(self):
        if self.attack_cooldown <= 0 and self.target:
            self.projectiles.append(Projectile(
                self.x, self.y, 
                self.target.x, self.target.y,
                5, self.attack_damage, self.team
            ))
            self.attack_cooldown = 60 // self.attack_speed
    
    def take_damage(self, damage):
        self.health -= damage
        if self.health <= 0:
            self.health = 0
            return True  # Unit died
        return False  # Unit still alive

# Specific monkey types
class DartMonkey(Monkey):
    def __init__(self, x, y, team):
        super().__init__(x, y, team)
        self.max_health = 80
        self.health = self.max_health
        self.attack_range = 200
        self.attack_speed = 1.5
        self.attack_damage = 8
        self.speed = 1.2
        self.cost = 100

class TackShooter(Monkey):
    def __init__(self, x, y, team):
        super().__init__(x, y, team)
        self.max_health = 120
        self.health = self.max_health
        self.attack_range = 120
        self.attack_speed = 0.8
        self.attack_damage = 6
        self.speed = 0.8
        self.cost = 150
        
    def attack(self):
        if self.attack_cooldown <= 0 and self.target:
            # Shoot in 8 directions
            for angle in range(0, 360, 45):
                rad = math.radians(angle)
                dx = math.cos(rad) * 5
                dy = math.sin(rad) * 5
                self.projectiles.append(Projectile(
                    self.x, self.y,
                    self.x + dx * 100, self.y + dy * 100,
                    5, self.attack_damage, self.team
                ))
            self.attack_cooldown = 60 // self.attack_speed

class SuperMonkey(Monkey):
    def __init__(self, x, y, team):
        super().__init__(x, y, team)
        self.max_health = 150
        self.health = self.max_health
        self.attack_range = 250
        self.attack_speed = 2.0
        self.attack_damage = 15
        self.speed = 1.0
        self.cost = 400
        
    def attack(self):
        if self.attack_cooldown <= 0 and self.target:
            for _ in range(3):  # Triple shot
                self.projectiles.append(Projectile(
                    self.x, self.y, 
                    self.target.x, self.target.y,
                    8, self.attack_damage, self.team
                ))
            self.attack_cooldown = 60 // self.attack_speed

class NinjaMonkey(Monkey):
    def __init__(self, x, y, team):
        super().__init__(x, y, team)
        self.max_health = 90
        self.health = self.max_health
        self.attack_range = 180
        self.attack_speed = 1.2
        self.attack_damage = 12
        self.speed = 2.0
        self.cost = 250
        
    def attack(self):
        if self.attack_cooldown <= 0 and self.target:
            self.projectiles.append(Projectile(
                self.x, self.y, 
                self.target.x, self.target.y,
                6, self.attack_damage, self.team
            ))
            self.attack_cooldown = 60 // self.attack_speed

class SpikeSpell:
    def __init__(self, x, y, team):
        self.x = x
        self.y = y
        self.team = team
        self.radius = 100
        self.duration = 180
        self.damage = 5
        self.timer = self.duration
        self.active = True
    
    def update(self, game_objects):
        self.timer -= 1
        if self.timer <= 0:
            self.active = False
            return
        
        # Damage all enemies in radius
        for obj in game_objects:
            if obj.team != self.team and hasattr(obj, 'health'):
                dist = math.hypot(self.x - obj.x, self.y - obj.y)
                if dist <= self.radius:
                    obj.take_damage(self.damage)
    
    def draw(self, surface):
        alpha = int(255 * (self.timer / self.duration))
        s = pygame.Surface((self.radius*2, self.radius*2), pygame.SRCALPHA)
        pygame.draw.circle(s, (0, 255, 255, alpha), (self.radius, self.radius), self.radius)
        surface.blit(s, (self.x - self.radius, self.y - self.radius))

class Game:
    def __init__(self):
        pygame.init()
        self.screen = pygame.display.set_mode((SCREEN_WIDTH, SCREEN_HEIGHT))
        pygame.display.set_caption("Monkey Clash")
        self.clock = pygame.time.Clock()
        self.running = True
        self.state = GameState.MENU
        
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
        start_x = (SCREEN_WIDTH - (5 * (card_width + padding))) // 2
        
        self.cards = [
            Card(start_x, SCREEN_HEIGHT - card_height - 10, 
                 card_width, card_height, 
                 CardType.MONKEY, "Dart", 100, 5),
            Card(start_x + (card_width + padding), SCREEN_HEIGHT - card_height - 10,
                 card_width, card_height,
                 CardType.MONKEY, "Tack", 150, 7),
            Card(start_x + 2 * (card_width + padding), SCREEN_HEIGHT - card_height - 10,
                 card_width, card_height,
                 CardType.MONKEY, "Super", 400, 15),
            Card(start_x + 3 * (card_width + padding), SCREEN_HEIGHT - card_height - 10,
                 card_width, card_height,
                 CardType.MONKEY, "Ninja", 250, 10),
            Card(start_x + 4 * (card_width + padding), SCREEN_HEIGHT - card_height - 10,
                 card_width, card_height,
                 CardType.SPELL, "Spike", 200, 20)
        ]
    
    def new_game(self):
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
                            # Place unit/spell if card selected
                            if 0 <= mouse_pos[0] <= SCREEN_WIDTH // 2:
                                if self.selected_card.name == "Dart":
                                    self.all_units.append(DartMonkey(mouse_pos[0], mouse_pos[1], Team.PLAYER))
                                elif self.selected_card.name == "Tack":
                                    self.all_units.append(TackShooter(mouse_pos[0], mouse_pos[1], Team.PLAYER))
                                elif self.selected_card.name == "Super":
                                    self.all_units.append(SuperMonkey(mouse_pos[0], mouse_pos[1], Team.PLAYER))
                                elif self.selected_card.name == "Ninja":
                                    self.all_units.append(NinjaMonkey(mouse_pos[0], mouse_pos[1], Team.PLAYER))
                                elif self.selected_card.name == "Spike":
                                    self.all_units.append(SpikeSpell(mouse_pos[0], mouse_pos[1], Team.PLAYER))
                                
                                if self.selected_card.use(mouse_pos[0], mouse_pos[1]):
                                    self.money -= self.selected_card.cost
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
                if isinstance(unit, Monkey):
                    unit.update([u for u in self.all_units if u != unit])
                    
                    # Collect projectiles
                    self.projectiles.extend(unit.projectiles)
                    unit.projectiles = []
                else:
                    unit.update([u for u in self.all_units if u.team != unit.team])
                
                # Check for unit death
                if hasattr(unit, 'health') and unit.health <= 0:
                    if unit.team == Team.ENEMY:
                        self.money += 50  # Reward for killing enemy
                    self.all_units.remove(unit)
                elif hasattr(unit, 'active') and not unit.active:
                    self.all_units.remove(unit)
            
            # Update projectiles and check for hits
            for proj in self.projectiles[:]:
                proj.update()
                
                # Check for hits
                for unit in self.all_units:
                    if (hasattr(unit, 'team') and unit.team != proj.team and 
                        hasattr(unit, 'health') and 
                        math.hypot(unit.x - proj.x, unit.y - proj.y) < 25):  # Hit radius
                        if unit.take_damage(proj.damage):
                            if unit.team == Team.ENEMY:
                                self.money += 50
                        proj.active = False
                        break
                
                if not proj.active:
                    self.projectiles.remove(proj)
            
            # Spawn enemy waves
            if pygame.time.get_ticks() % 3000 < 30:  # Every ~3 seconds
                self.spawn_enemy_wave()
    
    def spawn_enemy_wave(self):
        # Spawn 1-3 enemies
        for _ in range(random.randint(1, 3)):
            x = random.randint(SCREEN_WIDTH // 2, SCREEN_WIDTH - 50)
            y = random.randint(50, SCREEN_HEIGHT - 170)
            
            # Randomly choose enemy type
            monkey_type = random.choice([DartMonkey, TackShooter, SuperMonkey, NinjaMonkey])
            self.all_units.append(monkey_type(x, y, Team.ENEMY))
    
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
