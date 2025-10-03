import pygame
import math

class Monkey:
    def __init__(self, x, y, team):
        self.x = x
        self.y = y
        self.team = team
        self.level = 1
        self.xp = 0
        self.max_health = self.calculate_stats(100, 20)
        self.health = self.max_health
        self.attack_damage = self.calculate_stats(10, 2)
        self.attack_range = 150
        self.attack_speed = 1.0
        self.attack_cooldown = 0
        self.target = None
        self.projectiles = []
        self.radius = 20
        self.speed = 1.5

    def calculate_stats(self, base, per_level):
        return base + (self.level - 1) * per_level
        
    def gain_xp(self, amount):
        self.xp += amount
        if self.xp >= self.level * 100:
            self.level_up()
            
    def level_up(self):
        self.level += 1
        self.xp = 0
        self.max_health = self.calculate_stats(100, 20)
        self.health = self.max_health
        self.attack_damage = self.calculate_stats(10, 2)

    # [Rest of original Monkey methods remain the same]
