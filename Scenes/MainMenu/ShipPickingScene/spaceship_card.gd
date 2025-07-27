class_name SpaceshipCard
extends Panel

@onready var texture_button: TextureButton = $TextureButton
@onready var label: Label = $TextureButton/Label
var data: Dictionary

signal onSelected(data: Dictionary)

func set_data(d: Dictionary, name: String):
	data = d
	texture_button.texture_normal = load(d["icon"])
	label.text = name

func _on_texture_button_pressed() -> void:
	onSelected.emit(data, label.text)
