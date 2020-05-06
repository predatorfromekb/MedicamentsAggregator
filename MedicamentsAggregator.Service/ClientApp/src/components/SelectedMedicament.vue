<template>
    <section class="selected-medicament">
        <span class="selected-medicament__title">{{medicament.title}}</span>
        <div class="selected-medicament__buttons">
            <button class="selected-medicament__remove">➕</button>
            <button class="selected-medicament__remove">➖</button>
            <span class="selected-medicament__count">{{medicament.count}}</span>
        </div>
    </section>
</template>

<script>
    export default {
        name: "SelectedMedicament",
        props: {
            medicament: Object
        }
    }
</script>

<style scoped>
    .selected-medicament {
        display: grid;
        grid-template-columns: repeat(6, 1fr);
        
        white-space: nowrap;
        vertical-align: middle;
        
    }
    .selected-medicament__title {
        text-overflow: ellipsis;
        overflow: hidden;
        grid-column: span 5;
        padding-left: 5px;
    }

    .selected-medicament__count {
        text-align: center;
        border: 1px solid #777;
        grid-column: span 1;
        width: 100%;
        -webkit-appearance: none;
    }

    .selected-medicament__buttons {
        grid-column: span 1;
        display: grid;
        grid-template-columns: repeat(3, 1fr);
        grid-column-gap: 5px;
    }
    
</style>