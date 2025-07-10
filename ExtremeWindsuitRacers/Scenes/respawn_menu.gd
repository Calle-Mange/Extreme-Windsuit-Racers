extends Control

func _on_restart_pressed() -> void:
	get_tree().change_scene_to_file("res://Scenes/flight_test_level_Felix.tscn")
