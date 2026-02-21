<template>
    <h2 class="page-topic">Информация</h2>
    <table class="columns-journal">
        <thead>
            <tr>
                <td>Столб</td>
                <td>Статус</td>
                <td>Дата</td>
                <td>ID Дрона</td>
                <td>Время</td>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>LNTR_12345</td>
                <td>
                    <div class="td-wrapper">
                        <div class="indicator active"></div>
                        Работает
                    </div>
                </td>
                <td>09/12/25</td>
                <td>DRN_12345987</td>
                <td>30:00</td>
            </tr>
            <tr>
                <td>LNTR_17269</td>
                <td>
                    <div class="td-wrapper">
                        <div class="indicator inactive"></div>
                        Сломан
                    </div>
                </td>
                <td>09/12/25</td>
                <td>DRN_12348370</td>
                <td>30:00</td>
            </tr>
        </tbody>
    </table>
</template>

<script></script>

<style lang="scss">
.columns-journal {
    width: 100%;
    color: var(--color-text-grey);
    td {
        padding: 25px 0;
        border-bottom: 1px solid var(--color-border);
        .td-wrapper {
            display: flex;

            flex-direction: row;
            align-items: center;
            gap: 5px;
        }
    }
    .indicator {
        width: 10px;
        aspect-ratio: 1/1;
        border-radius: 50vi;
        background: var(--color-border);
        &.active {
            background: var(--color-green);
        }
        &.inactive{
            background: var(--color-red);
        }
    }
}
</style>
